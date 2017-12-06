namespace Fanex.ExternalAccount.Controllers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Context;
    using DTO;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.ViewModel.Shared;
    using Logging.Extensions.Web;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Service.Impl.Login;
    using Service.Interface.Login;
    using Utilities;

    public class SharedController : BaseController
    {
        private readonly ILoginService _loginService;
        private readonly CurrentSession _currentSession;
        private readonly IMaintenanceService _maintenanceService;

        public SharedController()
        {
            _currentSession = new CurrentSession();
            _loginService = new LoginService();
            _maintenanceService = new MaintenanceService();
        }

        public ActionResult _Layout()
        {
            return this.View();
        }

        public ActionResult Error()
        {
            ExceptionHandler.Handle(Server.GetLastError());

            return this.PartialView();
        }

        public ActionResult NoPermission()
        {
            return this.PartialView();
        }

        /// <summary>
        /// Gets menus.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult _Menu()
        {
            // load menus
            var menuService = new MenuService();
            var menuList = menuService.GetMenusByUserId(CurrentSession.UserId);
            var rootMenuList = menuList.Where(x => x.ParentID == 0);
            var menuDict = new Dictionary<string, MenuItem>();

            foreach (var item in rootMenuList)
            {
                var key = item.MenuName;
                var menuItem = new MenuItem
                {
                    Name = item.MenuName,
                    Url = item.Url,
                    Children = new List<MenuItem>()
                };

                var value = menuList.Where(x => x.ParentID != 0 && x.ParentID == item.MenuID).Select(
                    x => new MenuItem
                    {
                        Name = x.MenuName,
                        Url = x.Url
                    });

                menuItem.Children.AddRange(value.ToList());

                if (menuItem.Children.Count > 0 || !string.IsNullOrEmpty(menuItem.Url))
                {
                    menuDict.Add(key, menuItem);
                }
            }

            ViewBag.MenuDict = menuDict;

            return this.PartialViewBase();
        }

        public int ReCheckIn()
            => (int)GetRecheckinStatus();

        private RecheckinStatus GetRecheckinStatus()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                return RecheckinStatus.Maintenance;
            }

            if (!Config.IsEnabledRecheckin)
            {
                return RecheckinStatus.Continue;
            }

            int onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                try
                {
                    var isKickOut = false;
                    _loginService.Recheckin(onuserId, out isKickOut);
                    if (!isKickOut)
                    {
                        return RecheckinStatus.Continue;
                    }
                }
                catch (Exception ex)
                {
                    Logs.Error(ex);
                    return RecheckinStatus.Error;
                }
            }

            return RecheckinStatus.KickedOut;
        }

        public ActionResult Offline()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }

            if (_maintenanceService.IsUnderMaintenance())
            {
                UnderMaintenanceTime underMaintenanceTime = _maintenanceService.GetUnderMaintenanceTime();
                ViewBag.UmFromTime = underMaintenanceTime.From;
                ViewBag.UmToTime = underMaintenanceTime.To;
                return PartialView();
            }

            return RedirectToAction("Login", "Account");
        }

        public int ReCheckInOffline()
            => _maintenanceService.IsUnderMaintenance()
                ? (int)RecheckinStatus.Maintenance
                : (int)RecheckinStatus.Login;
    }
}
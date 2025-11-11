namespace Fanex.ExternalAccount.Controllers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Core.Context;
    using DTO;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.ViewModel.Shared;
    using Logging.Extensions.Web;
    using ONEbook.UM;
    using ONEbook.UM.Services;
    using Service.Impl.Login;
    using Service.Interface.Login;
    using Utilities;

    public class SharedController : BaseController
    {
        private static readonly string[] ignoreRecheckinCustomers = Config.IgnoreRecheckinCustomers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        private readonly ILoginService _loginService;
        private readonly CurrentSession _currentSession;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMenuService menuService;
        private readonly IGroupService groupService;

        public SharedController()
        {
            _currentSession = new CurrentSession();
            _loginService = new LoginService();
            _maintenanceService = new MaintenanceService(
                UMClientManager.RestClient,
                UMClientManager.CacheService,
                UMClientManager.UMClientConfigs,
                UMClientManager.HttpContext);
            menuService = new MenuService();
            groupService = new GroupService();
        }

        public IActionResult _Layout()
        {
            return this.View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return this.PartialView();
        }

        [AllowAnonymous]
        public IActionResult NoPermission()
        {
            return this.PartialView();
        }

        [AllowAnonymous]
        public IActionResult UnauthorizedView()
        {
            return this.PartialView("Unauthorized");
        }

        public IActionResult _Menu()
        {
            var groupId = CurrentSession.GroupId;
            var isMainGroupHasSubGroups = groupId > 0
                && (CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin || groupService.GetGroupWithSubGroups(groupId).Any());

            var isShowAssignSubGroup = isMainGroupHasSubGroups;
            var isShowSuperWinLossDetailBySubGroup = isMainGroupHasSubGroups;

            var menuList = menuService.GetMenusByUserId(
                CurrentSession.UserId,
                _currentSession.ParentGroupName,
                isShowAssignSubGroup,
                isShowSuperWinLossDetailBySubGroup);
            var rootMenuList = menuList.Where(x => x.ParentId == 0);
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

                var value = menuList.Where(x => x.ParentId != 0 && x.ParentId == item.MenuId).Select(
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
            if (!Config.IsEnabledRecheckin
                || ignoreRecheckinCustomers.Any(s => string.Equals(s, _currentSession.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                return RecheckinStatus.Continue;
            }

            int onuserId = _currentSession.OnUserId;
            if (onuserId > 0 && User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var isKickOut = _loginService.IsKickedOut(onuserId);
                    if (isKickOut)
                    {
                        HttpContext.Session.Clear();
                        HttpContext.SignOutAsync().Wait();
                    }
                    else
                    {
                        return RecheckinStatus.Continue;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                    return RecheckinStatus.Error;
                }
            }

            return RecheckinStatus.KickedOut;
        }

        [AllowAnonymous]
        public IActionResult Offline()
        {
            if (Request.Query.ContainsKey(Config.InternalAgentUMToken))
            {
                HttpContext.Session.SetString(Config.InternalAgentUMToken, Request.Query[Config.InternalAgentUMToken].ToString());
            }

            if (_maintenanceService.IsUnderMaintenance())
            {
                var underMaintenanceTime = _maintenanceService.GetUnderMaintenanceTime();
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

        public bool ChangeSelectedGroup(string groupName)
        {
            if (CurrentSession.IsInternalAdmin)
            {
                var group = groupService.GetGroupByName(groupName);

                if (group != null)
                {
                    _currentSession.ParentGroupName = group.ParentGroupId == 0 ? groupName : group.ParentGroupName;
                }
            }

            CurrentSession.CurrencyGroup = groupName;

            return true;
        }
    }
}
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
    using ONEbook.UM.Services;
    using Service.Interface.Login;
    using Utilities;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class SharedController : BaseController
    {
        private static readonly string[] ignoreRecheckinCustomers = Config.IgnoreRecheckinCustomers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        private readonly ILoginService loginService;
        private readonly IMaintenanceService maintenanceService;
        private readonly IMenuService menuService;
        private readonly IGroupService groupService;

        public SharedController(ILoginService loginService, IMaintenanceService maintenanceService, IMenuService menuService, IGroupService groupService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, Core.Authorization.IAuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.loginService = loginService;
            this.maintenanceService = maintenanceService;
            this.menuService = menuService;
            this.groupService = groupService;
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
                CurrentSession.ParentGroupName,
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
                || ignoreRecheckinCustomers.Any(s => string.Equals(s, CurrentSession.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                return RecheckinStatus.Continue;
            }

            int onuserId = CurrentSession.OnUserId;
            if (onuserId > 0 && User.Identity?.IsAuthenticated == true)
            {
                try
                {
                        var isKickOut = loginService.IsKickedOut(onuserId);
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

            if (maintenanceService.IsUnderMaintenance())
            {
                var underMaintenanceTime = maintenanceService.GetUnderMaintenanceTime();
                ViewBag.UmFromTime = underMaintenanceTime.From;
                ViewBag.UmToTime = underMaintenanceTime.To;
                return PartialView();
            }

            return RedirectToAction("Login", "Account");
        }

        public int ReCheckInOffline()
            => maintenanceService.IsUnderMaintenance()
                ? (int)RecheckinStatus.Maintenance
                : (int)RecheckinStatus.Login;

        public bool ChangeSelectedGroup(string groupName)
        {
            if (CurrentSession.IsInternalAdmin)
            {
                var group = groupService.GetGroupByName(groupName);

                if (group != null)
                {
                    CurrentSession.ParentGroupName = group.ParentGroupId == 0 ? groupName : group.ParentGroupName;
                }
            }

            CurrentSession.CurrencyGroup = groupName;

            return true;
        }
    }
}
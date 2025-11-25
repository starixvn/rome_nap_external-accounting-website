namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using Service.Impl.UserControl;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly ICachingService cachingService;

        public MenuController(ICachingService cachingService, IMenuService menuService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.menuService = menuService;
            this.cachingService = cachingService;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult MenuList() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult MenuListPost()
        {
            var result = GetAllMenus();

            return JsonBase(new { total = 1, page = 1, records = result.Count, rows = result },
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult AddMenu()
        {
            ViewBag.FunctionList = GetFunctions();
            ViewBag.MenuList = GetMenus();

            return PartialViewBase(new Menu());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult EditMenu(int id)
        {
            var model = menuService.GetAllMenus().FirstOrDefault(o => o.MenuId == id);

            if (model != null)
            {
                model.MenuName = WebUtility.HtmlDecode(model.MenuName);
                model.MenuDesc = WebUtility.HtmlDecode(model.MenuDesc);
                model.Url = WebUtility.HtmlDecode(model.Url);

                ViewBag.FunctionList = GetFunctions();
                ViewBag.MenuList = GetMenus();
            }

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Menu menu)
        {
            if (menu != null)
            {
                menu.IsActive = true;
            }
            var result = menuService.CreateMenu(menu, CurrentSession.CustId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Menu menu)
        {
            var result = menuService.UpdateMenu(menu, CurrentSession.CustId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int menuId, bool isActive)
        {
            var result = menuService.UpdateMenuStatus(
                menuId,
                isActive,
                CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int menuId)
        {
            var result = menuService.DeleteMenu(
                menuId,
                CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetAllMenus()
        {
            var menuList = menuService.GetAllMenus();
            var parents = menuList.Where(x => x.ParentId == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentId != 0 && x.ParentId == parent.MenuId);
                if (items.Any())
                {
                    list.AddRange(items);
                }
            }

            return list;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetMenus()
        {
            var menuList = cachingService.GetUserCache(CurrentSession.UserId, ExtRequest.GetGroupName()).MenuList;
            var parents = menuList.Where(x => x.ParentId == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
            }

            return list;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Function> GetFunctions()
        {
            var menuList = cachingService.GetUserCache(CurrentSession.UserId, ExtRequest.GetGroupName()).PolicyList;
            var parents = menuList.Where(x => x.ParentID == 0);
            var list = new List<Function>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentID != 0 && x.ParentID == parent.FuncID);
                if (items.Any())
                {
                    list.AddRange(items);
                }
            }

            return list;
        }
    }
}
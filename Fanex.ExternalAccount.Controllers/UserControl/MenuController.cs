// --------------------------------------------------------------------------
// <copyright file="MenuController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService = null;

        public MenuController()
        {
            this._menuService = this._menuService ?? new MenuService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult MenuList()
        {
            return this.ViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult MenuListPost()
        {
            var result = this.GetAllMenus();

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddMenu()
        {
            var userCache = new CachingService().GetUserCache(CurrentSession.UserId);

            ViewBag.FunctionList = this.GetFunctions();
            ViewBag.MenuList = this.GetMenus();

            return this.PartialViewBase(new Menu());
        }

        /// <summary>
        /// Gets edit menu page.
        /// </summary>
        /// <param name="id">Id of menu.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditMenu(int id)
        {
            var model = this._menuService.GetAllMenus().Where(o => o.MenuID == id).FirstOrDefault();
            var userCache = new CachingService().GetUserCache(CurrentSession.UserId);

            model.MenuName = HttpUtility.HtmlDecode(model.MenuName);
            model.MenuDesc = HttpUtility.HtmlDecode(model.MenuDesc);
            model.Url = HttpUtility.HtmlDecode(model.Url);

            ViewBag.FunctionList = this.GetFunctions();
            ViewBag.MenuList = this.GetMenus();

            return this.PartialViewBase(model);
        }

        /// <summary>
        /// Add new menu.
        /// </summary>
        /// <param name="name">Menu name.</param>
        /// <param name="description">Menu description.</param>
        /// <param name="url">Menu's URL.</param>
        /// <param name="functionId">Function id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder)
        {
            var menuId = 0;

            var result = this._menuService.CreateMenu(
                    name,
                    description,
                    url,
                    functionId,
                    parentId,
                    parentName,
                    displayOrder,
                    true,
                    CurrentSession.CustId,
                    CurrentContext.Ip4Address,
                    out menuId);

            return this.JsonBase(result);
        }

        /// <summary>
        /// Edits menu item.
        /// </summary>
        /// <param name="menuId">Id of menu.</param>
        /// <param name="name">Menu name.</param>
        /// <param name="description">Menu description.</param>
        /// <param name="url">Menu's URL.</param>
        /// <param name="functionId">Function id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(int menuId, string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder)
        {
            var result = this._menuService.UpdateMenu(
                    menuId,
                    name,
                    description,
                    url,
                    functionId,
                    parentId,
                    parentName,
                    displayOrder,
                    CurrentSession.CustId,
                    CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateStatus(int menuId, bool isActive)
        {
            var result = this._menuService.UpdateMenuStatus(
                    menuId,
                    isActive,
                    CurrentSession.CustId);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Delete(int menuId)
        {
            var result = this._menuService.DeleteMenu(
                    menuId,
                    CurrentSession.CustId);

            return this.JsonBase(result);
        }

        /// <summary>
        /// Gets all menus.
        /// </summary>
        /// <returns>List of menus.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetAllMenus()
        {
            var menuList = this._menuService.GetAllMenus();
            var parents = menuList.Where(x => x.ParentID == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentID != 0 && x.ParentID == parent.MenuID);
                if (items.Count() > 0)
                {
                    list.AddRange(items);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets list of menus.
        /// </summary>
        /// <returns>List of menus.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Menu> GetMenus()
        {
            var menuList = new CachingService().GetUserCache(CurrentSession.UserId).MenuList;
            var parents = menuList.Where(x => x.ParentID == 0);
            var list = new List<Menu>();
            foreach (var parent in parents)
            {
                list.Add(parent);
            }

            return list;
        }

        /// <summary>
        /// Gets list of functions.
        /// </summary>
        /// <returns>List of functions.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageMenu, Policy = AuthenticateAction.AuthenticationType.Read)]
        private List<Function> GetFunctions()
        {
            var menuList = new CachingService().GetUserCache(CurrentSession.UserId).PolicyList;
            var parents = menuList.Where(x => x.ParentID == 0);
            var list = new List<Function>();
            foreach (var parent in parents)
            {
                list.Add(parent);
                var items = menuList.Where(x => x.ParentID != 0 && x.ParentID == parent.FuncID);
                if (items.Count() > 0)
                {
                    list.AddRange(items);
                }
            }

            return list;
        }
    }
}
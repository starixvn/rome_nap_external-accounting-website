// --------------------------------------------------------------------------
// <copyright file="MenuController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MenuController : BaseController
    {
        private readonly IMenuService menuService = null;

        public MenuController()
        {
            this.menuService = this.menuService ?? new MenuService();
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        public ActionResult MenuList()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        public ActionResult MenuListPost()
        {
            var result = this.GetAllMenus();

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result });
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        public ActionResult AddMenu()
        {
            var userCache = new CachingService().GetUserCache(CurrentSession.UserId);

            ViewBag.FunctionList = this.GetFunctions();
            ViewBag.MenuList = this.GetMenus();

            return this.PartialViewBase(new Menu());
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        public ActionResult EditMenu(int id)
        {
            var model = this.menuService.GetAllMenus().Where(o => o.MenuID == id).FirstOrDefault();
            var userCache = new CachingService().GetUserCache(CurrentSession.UserId);

            ViewBag.FunctionList = this.GetFunctions();
            ViewBag.MenuList = this.GetMenus();

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Create)]
        public ActionResult Add(string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder)
        {
            var menuId = 0;

            var result = this.menuService.CreateMenu(
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

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Update)]
        public ActionResult Edit(int menuId, string name, string description, string url, int functionId, int parentId, string parentName, int displayOrder)
        {
            var result = this.menuService.UpdateMenu(
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

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Update)]
        public ActionResult UpdateStatus(int menuId, bool isActive)
        {
            var result = this.menuService.UpdateMenuStatus(
                    menuId,
                    isActive,
                    CurrentSession.CustId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Delete)]
        public ActionResult Delete(int menuId)
        {
            var result = this.menuService.DeleteMenu(
                    menuId,
                    CurrentSession.CustId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        private List<Menu> GetAllMenus()
        {
            var menuList = this.menuService.GetAllMenus();
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

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
        private List<Menu> GetMenus()
        {
            var menuList = new CachingService().GetUserCache(CurrentSession.UserId).MenuList;
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

        [Authorization(Function = eFunctions.ManageMenu, Policy = ePolicy.Read)]
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
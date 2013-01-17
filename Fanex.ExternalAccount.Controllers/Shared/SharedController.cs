// --------------------------------------------------------------------------
// <copyright file="SharedController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Shared controller
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.5.23">Format styles</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.ViewModel.Shared;
    using Fanex.ExternalAccount.Controllers.Base;

    /// <summary>
    /// Shared controller object
    /// </summary>
    public class SharedController : BaseController
    {
        public ActionResult _Layout()
        {
            return this.View();
        }

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
    }
}
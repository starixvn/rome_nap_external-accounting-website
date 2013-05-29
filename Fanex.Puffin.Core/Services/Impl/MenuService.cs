// --------------------------------------------------------------------------
// <copyright file="MenuService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Fanex.NAP.Core.Model;
using Fanex.Puffin.Core.Web;

namespace Fanex.Puffin.Core.Services.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class MenuService : ServiceBase
    {
        public const char URL_SEPARATOR = '/';

        public MenuItem GetMenuById(int menuId)
        {
            try
            {
                if (AppSession.CurrentUserMenu != null)
                {
                    MenuItem menu = AppSession.CurrentUserMenu
                          .FirstOrDefault<MenuItem>(m => m.Id == menuId);
                    return menu;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return null;
        }

        public MenuItem GetMenuByURL(string menuURL)
        {
            try
            {
                if (!string.IsNullOrEmpty(menuURL) &&
                    !string.Equals(menuURL, URL_SEPARATOR.ToString()) &&
                    AppSession.CurrentUserMenu != null)
                {
                    MenuItem menu = AppSession.CurrentUserMenu
                        .FirstOrDefault<MenuItem>(m => m.Url.StartsWith(menuURL));
                    return menu;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return null;
        }

        public MenuItem GetMenuByURL(string menuURL, IList<MenuItem> menuList)
        {
            try
            {
                if (!string.IsNullOrEmpty(menuURL) &&
                    !string.Equals(menuURL, URL_SEPARATOR.ToString()) &&
                    AppSession.CurrentUserMenu != null)
                {
                    MenuItem menu = menuList.FirstOrDefault<MenuItem>(m => m.Url.StartsWith(menuURL));
                    return menu;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return null;
        }

        public string GetMenuNameByURL(string menuURL)
        {
            try
            {
                if (!string.IsNullOrEmpty(menuURL) &&
                    !string.Equals(menuURL, URL_SEPARATOR.ToString()) &&
                    AppSession.CurrentUserMenu != null)
                {
                    MenuItem menu = AppSession.CurrentUserMenu
                        .FirstOrDefault<MenuItem>(m => m.Url.StartsWith(menuURL));
                    return menu.Name;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return null;
        }

        public static string GetMenuPathFromRequestFilePath(string filePath,
            string applicationPath)
        {
            filePath = filePath.Trim();
            applicationPath = applicationPath.Trim();
            var menuPath = filePath;
            if (!string.IsNullOrEmpty(applicationPath) &&
                menuPath.StartsWith(applicationPath) &&
                AppSession.CurrentUserMenu != null)
            {
                menuPath = menuPath.Remove(0, applicationPath.Length);
            }

            if (!menuPath.StartsWith(URL_SEPARATOR.ToString()))
            {
                menuPath = URL_SEPARATOR + menuPath;
            }

            return menuPath;
        }
    }
}

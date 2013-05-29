// --------------------------------------------------------------------------
// <copyright file="AppSession.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.10.18">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.22">Update Session Manager</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.NAP.Core.Model;

namespace Fanex.Puffin.Core.Web
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class AppSession
    {
        /// <summary>
        /// Gets or sets current login user
        /// </summary>
        public static User CurrentUser
        {
            get
            {
                return SessionManager.Get<User>(Constants.CURRENT_LOG_IN_USER);
            }

            set
            {
                SessionManager.Set<User>(Constants.CURRENT_LOG_IN_USER, value);
            }
        }

        /// <summary>
        /// Gets or sets menu of current login user
        /// </summary>
        public static IList<MenuItem> CurrentUserMenu
        {
            get
            {
                return SessionManager.Get<IList<MenuItem>>(Constants.CURRENT_USER_MENU);
            }

            set
            {
                SessionManager.Set<IList<MenuItem>>(Constants.CURRENT_USER_MENU, value);
            }
        }

        public static Dictionary<string, int> RoleAccess
        {
            get
            {
                try
                {
                    return SessionManager.Get<Dictionary<string, int>>(Constants.CURRENT_USER_ROLE_ACCESS);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            set
            {
                SessionManager.Set<Dictionary<string, int>>(Constants.CURRENT_USER_ROLE_ACCESS, value);
            }
        }

        /// <summary>
        /// Gets or sets current working menu url
        /// </summary>
        public static MenuItem CurrentWorkingOnMenu
        {
            get
            {
                return SessionManager.Get<MenuItem>(Constants.CURRENT_WORKING_ON_MENU);
            }

            set
            {
                SessionManager.Set<MenuItem>(Constants.CURRENT_WORKING_ON_MENU, value);
            }
        }


        /// <summary>
        /// Gets or sets IsTrackedMenu: determine current working on menu is
        /// tracked for page tracking or not
        /// </summary>
        public static string CurrentActionName
        {
            get
            {
                return SessionManager.Get<string>(Constants.CURRENT_ACTION_NAME);
            }

            set
            {
                SessionManager.Set<string>(Constants.CURRENT_ACTION_NAME, value);
            }
        }

        public static bool ViewIPVN
        {
            get
            {
                return SessionManager.Get<bool>(Constants.VIEW_IP_VN);
            }
            set
            {
                SessionManager.Set<bool>(Constants.VIEW_IP_VN, value);
            }
        }
    }
}
// --------------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.20">Update Check Permission</change>
// </history>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.NAP.Core.Business;
using Fanex.NAP.Core.Model.ValueObject;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.Puffin.Core.Web;

namespace Fanex.Puffin.Core.Services.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class PermissionService
    {
        private static readonly IUserService _userService;

        static PermissionService()
        {
            _userService = new UserService();
        }

        /// <summary>
        /// GetRollAccess: get permission to access with each menu item of current user login
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetRoleAccess(IEnumerable<FunctionPolicy> currentFunctions)
        {
            Dictionary<string, int> roleAccess = new Dictionary<string, int>();

            foreach (FunctionPolicy functionPolicy in currentFunctions)
            {
                if (functionPolicy.Policy >= (int)Constant.Policy.Read)
                {
                    roleAccess.Add(functionPolicy.Function.Key, functionPolicy.Policy);
                }
            }

            return roleAccess;
        }

        /// <summary>
        /// HasPermission: check that user has permission to access one menu link
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public static bool HasReadPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] == (int)Constant.Policy.Read;
        }

        public static bool HasHostPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] == (int)Constant.Policy.Host;
        }

        public static bool HasPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] >= (int)Constant.Policy.Read;
        }
    }
}

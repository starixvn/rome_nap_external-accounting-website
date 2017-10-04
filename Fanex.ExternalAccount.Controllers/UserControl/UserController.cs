// --------------------------------------------------------------------------
// <copyright file="UserController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// User controller
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Services.Impl;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserService _userService = new UserService();
        private readonly CachingService _cache = new CachingService();
        private enum AlphaSiteCode
        {
            I1 = 1,
            I7 = 7,
            I9 = 9
        }

        /// <summary>
        /// Gets user list.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult UserList()
        {
            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup();
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup().Where(o => o.GroupID != 1);
            }
            else
            {
                ViewBag.GroupList = new GroupService().GetListGroup().Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets user list data.
        /// </summary>
        /// <param name="userName">Username of customer.</param>
        /// <param name="groupId">Group id that user belongs to.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult UserListPost(string userName = null, int? groupId = null)
        {
            var result = this._userService.GetListUsers(userName, groupId);

            if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupID != 1);
            }
            else if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }

        /// <summary>
        /// Get edit user page.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditUser(int id)
        {
            var model = this._userService.GetUserById(id);

            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);

            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup();
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup().Where(o => o.GroupID != 1);
            }
            else
            {
                ViewBag.GroupList = new LoginService().GroupList();
            }

            return this.PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(int custId, string firstName, string lastName, int groupId)
        {
            var result = this._userService.EditUser(CurrentSession.UserId, custId, firstName, lastName, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        /// <summary>
        /// Gets add new user page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddUser()
        {
            var model = new User();

            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup();
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup()
                    .Where(o => o.GroupID != 1);
            }
            else
            {
                ViewBag.GroupList = new GroupService().GetListGroup()
                    .Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return this.PartialViewBase(model);
        }

        /// <summary>
        /// Gets add existing user page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddExistUser()
        {
            var model = new User();

            if (CurrentSession.IsITAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup();
            }
            else if (CurrentSession.IsInternalAdmin)
            {
                ViewBag.GroupList = new GroupService().GetListGroup()
                    .Where(o => o.GroupID != 1);
            }
            else
            {
                ViewBag.GroupList = new GroupService().GetListGroup()
                    .Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return this.PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(string username, string pwd, string firstName, string lastName, int groupId)
        {
            var result = this._userService.CreateUser(CurrentSession.UserId, username, pwd, firstName, lastName, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult AddExist(string username, int groupId)
        {
            var result = this._userService.CreateExistUser(CurrentSession.UserId, username, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateStatus(int userId, bool isActive)
        {
            var result = this._userService.UpdateStatus(CurrentSession.UserId, userId, isActive, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetPwd(int userId, string pwd)
        {
            var result = this._userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult ResetPassword(int id)
        {
            var model = this._userService.GetUserById(id);

            ViewBag.IsInternalAdmin = new LoginService().CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return this.PartialViewBase(model);
        }

        #region Reset password for current logging in user

        [AntiXSRFFilter]
        public ActionResult ResetPwdOwner(int userId, string pwd)
        {
            if (userId != CurrentSession.UserId)
            {
                return this.ActionDenied(this.Request);
            }

            var result = this._userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        public ActionResult ResetPasswordOwner(int id)
        {
            if (id != CurrentSession.UserId)
            {
                return this.ActionDenied(this.Request);
            }

            var model = this._userService.GetUserById(id);
            ViewBag.IsInternalAdmin = new LoginService().CheckExternalGroup(id, null, eFunctions.ExternalGroup);
            return this.PartialViewBase(model);
        }

        #endregion

        /// <summary>
        /// Gets grant permission page.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="username">Admin username.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>ActionResult object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GrantPermission(int userId, string username, string groupName)
        {
            var funcs = new FunctionService().GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var groups = new GroupService().GetListGroup();
            var externalGroup = new LoginService().GroupList();
            var functionList = this._userService.GetUserFunction(userId);
            var allowFunctions = new List<Function>();

            if (!IncludeAllFunctions)
            {
                foreach (var function in functionList)
                {
                    if (!funcs.Any(f => f.FuncID == function.FuncID))
                    {
                        allowFunctions.Add(function);
                    }
                }

                functionList = allowFunctions;
            }

            var groupId = groups.Where(o => o.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().GroupID;

            ViewBag.UserID = userId;
            ViewBag.Username = username;

            var isEditExtGroup = false;

            foreach (var item in externalGroup)
            {
                if (item.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                {
                    isEditExtGroup = true;
                    break;
                }
            }

            if (isEditExtGroup)
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (groupId != 1)
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction &&
                        x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }

            return this.PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageUser, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult GrantPermissionPost(int userId, string oldPolicy, string policy)
        {
            var result = this._userService.UpdateUserPolicy(CurrentSession.CustId, userId, oldPolicy, policy, CurrentContext.Ip4Address);
            if (result.IsSuccess)
            {
                this._cache.RemoveUserCache(CurrentSession.UserId);
            }

            return this.JsonBase(result);
        }

        [HttpPost]
        public ActionResult InternalLogin(string username)
        {
            var url = string.Empty;
            var checkCustomerBelongToSpecialSiteResult = base.CheckCustomerBelongToSpecialSite(username);
            if (base.IsAdminHasPermissionOnCustomer(username, checkCustomerBelongToSpecialSiteResult.IsSpecialSite))
            {
                var encryption = new RSACryptoPad.EncryptionThread();
                var param = string.Join("&", username, this.CurrentSession.UserId, new LoginService().GetDBTime().Ticks, "NAPEXT");
                param = encryption.EncryptString(param, 1024, Request.PhysicalApplicationPath + "/IbcKeys/ibcpkey.snk");

                url = string.Format(
                "{0}?{1}params={2}",
                GetInternalLoginUrl((AlphaSiteCode)checkCustomerBelongToSpecialSiteResult.SiteCode),
                Session[Config.InternalAgentUMToken] == null ? string.Empty : string.Format("UMToken={0}&", Session[Config.InternalAgentUMToken]),
                    param);
            }
            else
            {
                url = Url.Content("~") + "Shared/NoPermission";
            }

            return this.JsonBase(new { errorCode = 0, url = url });
        }
        private static string GetInternalLoginUrl(AlphaSiteCode siteCode)
        {
            switch (siteCode)
            {
                case AlphaSiteCode.I7:
                    return Config.InternalLoginUrl7;
                case AlphaSiteCode.I9:
                    return Config.InternalLoginUrl9;
                default:
                    return Config.InternalLoginUrl;
            }
        }
    }
}
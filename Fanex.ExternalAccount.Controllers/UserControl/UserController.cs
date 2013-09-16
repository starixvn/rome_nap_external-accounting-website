// --------------------------------------------------------------------------
// <copyright file="UserController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserService _userService = new UserService();

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read, IsAjaxRequest = false)]
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

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
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

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result });
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
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

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult Edit(int custId, string firstName, string lastName, int groupId)
        {
            var result = this._userService.EditUser(CurrentSession.UserId, custId, firstName, lastName, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
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

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
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

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Create)]
        public ActionResult Add(string username, string pwd, string firstName, string lastName, int groupId)
        {
            var result = this._userService.CreateUser(CurrentSession.UserId, username, pwd, firstName, lastName, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Create)]
        public ActionResult AddExist(string username, int groupId)
        {
            var result = this._userService.CreateExistUser(CurrentSession.UserId, username, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult UpdateStatus(int userId, bool isActive)
        {
            var result = this._userService.UpdateStatus(CurrentSession.UserId, userId, isActive, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult ResetPwd(int userId, string pwd)
        {
            var result = this._userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
        public ActionResult ResetPassword(int id)
        {
            var model = this._userService.GetUserById(id);

            ViewBag.IsInternalAdmin = new LoginService().CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return this.PartialViewBase(model);
        }

        #region Reset password for current logging in user
                
        public ActionResult ResetPwdOwner(int userId, string pwd)
        {
            var result = this._userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        public ActionResult ResetPasswordOwner(int id)
        {
            var model = this._userService.GetUserById(id);

            ViewBag.IsInternalAdmin = new LoginService().CheckExternalGroup(id, null, eFunctions.ExternalGroup);

            return this.PartialViewBase(model);
        }

        #endregion

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
        public ActionResult GrantPermission(int userId, string username, string groupName)
        {
            var groups = new GroupService().GetListGroup();
            var externalGroup = new LoginService().GroupList();
            var functionList = this._userService.GetUserFunction(userId);
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
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 &&
                    x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup &&
                    x.FuncKey != eFunctions.ManageMenu &&
                    x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration &&
                    x.FuncKey != eFunctions.SystemConfiguration &&
                    x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup &&
                    x.FuncKey != eFunctions.ManageMenu &&
                    x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration &&
                    x.FuncKey != eFunctions.SystemConfiguration &&
                    x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (groupId != 1)
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 &&
                        x.FuncKey != eFunctions.ManageFunction &&
                        x.FuncKey != eFunctions.ManageMenu &&
                        x.FuncKey != eFunctions.SystemConfiguration);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageMenu &&
                    x.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0);
                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0);
            }

            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult GrantPermissionPost(int userId, string oldPolicy, string policy)
        {
            var result = this._userService.UpdateUserPolicy(CurrentSession.CustId, userId, oldPolicy, policy, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [HttpPost]
        public ActionResult InternalLogin(string username)
        {
            var encryption = new RSACryptoPad.EncryptionThread();
            var param = string.Join("&", username, this.CurrentSession.UserId, new LoginService().GetDBTime().Ticks);
            param = encryption.EncryptString(param, 1024, Request.PhysicalApplicationPath + "/IbcKeys/ibcpkey.snk");
            var url = ConfigurationManager.AppSettings["int_agent_url"].ToString() + "?params=" + param;

            return this.JsonBase(new { errorCode = 0, url = url });
        }
    }
}
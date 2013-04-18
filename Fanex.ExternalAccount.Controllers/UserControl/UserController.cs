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
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
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
        private readonly IUserService userService = new UserService();

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
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

            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
        public ActionResult UserListPost(string userName = null, int? groupId = null)
        {
            var result = this.userService.GetListUsers(userName, groupId);

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
            var model = this.userService.GetUserById(id);

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
            var result = this.userService.EditUser(CurrentSession.UserId, custId, firstName, lastName, CurrentContext.Ip4Address, groupId);

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
            var result = this.userService.CreateUser(CurrentSession.UserId, username, pwd, firstName, lastName, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Create)]
        public ActionResult AddExist(string username, int groupId)
        {
            var result = this.userService.CreateExistUser(CurrentSession.UserId, username, groupId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult UpdateStatus(int userId, bool isActive)
        {
            var result = this.userService.UpdateStatus(CurrentSession.UserId, userId, isActive, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Update)]
        public ActionResult ResetPwd(int userId, string pwd)
        {
            var result = this.userService.ResetPassword(CurrentSession.UserId, userId, pwd, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.Reports, Policy = ePolicy.Read)]
        public ActionResult ResetPassword(int id)
        {
            var model = this.userService.GetUserById(id);

            ViewBag.IsInternalAdmin = new LoginService().CheckExternalGroup(id, (int)eFunctions.ExternalGroup, eFunctions.ExternalGroup.ToString());

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageUser, Policy = ePolicy.Read)]
        public ActionResult GrantPermission(int userId, string username, string groupName)
        {
            var groups = new GroupService().GetListGroup();
            var externalGroup = new LoginService().GroupList();
            var functionList = this.userService.GetUserFunction(userId);
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
                    x.FuncID != (int)eFunctions.ManageFunction &&
                    x.FuncID != (int)eFunctions.ManageGroup &&
                    x.FuncID != (int)eFunctions.ManageMenu &&
                    x.FuncID != (int)eFunctions.ManageUser &&
                    x.FuncID != (int)eFunctions.Administration &&
                    x.FuncID != (int)eFunctions.SystemConfiguration &&
                    x.FuncID != (int)eFunctions.ExternalGroup);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncID != (int)eFunctions.ManageFunction &&
                    x.FuncID != (int)eFunctions.ManageGroup &&
                    x.FuncID != (int)eFunctions.ManageMenu &&
                    x.FuncID != (int)eFunctions.ManageUser &&
                    x.FuncID != (int)eFunctions.Administration &&
                    x.FuncID != (int)eFunctions.SystemConfiguration &&
                    x.FuncID != (int)eFunctions.ExternalGroup);
            }
            else if (groupId != 1)
            {
                ViewBag.UserFunc = functionList.Where(x => x.ParentID == 0 &&
                        x.FuncID != (int)eFunctions.ManageFunction &&
                        x.FuncID != (int)eFunctions.ManageMenu &&
                        x.FuncID != (int)eFunctions.SystemConfiguration);

                ViewBag.UserSubFunc = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncID != (int)eFunctions.ManageFunction &&
                    x.FuncID != (int)eFunctions.ManageMenu &&
                    x.FuncID != (int)eFunctions.SystemConfiguration);
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
            var result = this.userService.UpdateUserPolicy(CurrentSession.CustId, userId, oldPolicy, policy, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }
    }
}
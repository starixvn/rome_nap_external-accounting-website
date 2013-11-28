// --------------------------------------------------------------------------
// <copyright file="GroupController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupController : BaseController
    {
        private IGroupService _groupService = null;
        private CachingService _cache = null;

        public GroupController()
        {
            this._groupService = this._groupService ?? new GroupService();
            this._cache = this._cache ?? new CachingService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GroupList()
        {
            return this.PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult AddGroup()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets group list data.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GroupListPost()
        {
            var result = this._groupService.GetListGroup();

            if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }
            else if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupID != 1);
            }

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditGroup(int id)
        {
            var model = this._groupService.GetGroupById(id);

            model.GroupName = HttpUtility.HtmlDecode(model.GroupName);
            model.Description = HttpUtility.HtmlDecode(model.Description);

            return this.PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GetGroupPolicy(int groupId)
        {
            var model = this._groupService.GetAllGroupPolicy(groupId);

            return this.PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult Add(string name, string description)
        {
            var groupId = 0;

            var result = this._groupService.CreateGroup(name, description, CurrentSession.CustId, CurrentContext.Ip4Address, out groupId);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult EditPolicy(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = this._groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult Delete(int groupId)
        {
            var result = this._groupService.DeleteGroup(groupId, CurrentSession.CustId);

            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult Edit(int groupId, string groupName, string description)
        {
            var result = this._groupService.EditGroup(groupName, description, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        /// <summary>
        /// Grants permission for group.
        /// </summary>
        /// <param name="groupId">Id of group.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GrantPermission(int groupId, string groupName)
        {
            var externalGroup = new LoginService().GroupList();
            var functionList = this._groupService.GetAllGroupPolicy(groupId);
            var isEditExtGroup = false;
            ViewBag.GroupID = groupId;
            ViewBag.GroupName = groupName;

            foreach (var item in externalGroup)
            {
                if (item.GroupName.Equals(groupName, System.StringComparison.OrdinalIgnoreCase))
                {
                    isEditExtGroup = true;
                    break;
                }
            }

            if (!isEditExtGroup) 
            {
                // Continue to check new external groups created after launching
                isEditExtGroup = groupId != 1 && !groupName.Equals("Finance", System.StringComparison.OrdinalIgnoreCase) && 
                    !groupName.Equals("Internal", System.StringComparison.OrdinalIgnoreCase);
            }

            if (isEditExtGroup)
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (groupId != 1)
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction &&
                        x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }

            return this.PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult GrantPermissionPost(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = this._groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);
            if (result.IsSuccess)
            {
                this._cache.RemoveUserCache(CurrentSession.UserId);
            }

            return this.JsonBase(result);
        }
    }
}
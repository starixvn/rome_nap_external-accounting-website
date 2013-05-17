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
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupController : BaseController
    {
        private IGroupService groupService = null;

        public GroupController()
        {
            this.groupService = this.groupService ?? new GroupService();
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult GroupList()
        {
            return this.ViewBase();
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult AddGroup()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult GroupListPost()
        {
            var result = this.groupService.GetListGroup();

            if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }
            else if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupID != 1);
            }

            return this.JsonBase(new { total = 1, page = 1, records = result.Count(), rows = result });
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult EditGroup(int id)
        {
            var model = this.groupService.GetGroupById(id);

            model.GroupName = HttpUtility.HtmlDecode(model.GroupName);
            model.Description = HttpUtility.HtmlDecode(model.Description);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult GetGroupPolicy(int groupId)
        {
            var model = this.groupService.GetAllGroupPolicy(groupId);

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Create)]
        public ActionResult Add(string name, string description)
        {
            var groupId = 0;

            var result = this.groupService.CreateGroup(name, description, CurrentSession.CustId, CurrentContext.Ip4Address, out groupId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Update)]
        public ActionResult EditPolicy(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = this.groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Delete)]
        public ActionResult Delete(int groupId)
        {
            var result = this.groupService.DeleteGroup(groupId, CurrentSession.CustId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Update)]
        public ActionResult Edit(int groupId, string groupName, string description)
        {
            var result = this.groupService.EditGroup(groupName, description, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult GrantPermission(int groupId, string groupName)
        {
            var externalGroup = new LoginService().GroupList();
            var functionList = this.groupService.GetAllGroupPolicy(groupId);

            ViewBag.GroupID = groupId;
            ViewBag.GroupName = groupName;

            var isEditExtGroup = false;

            foreach (var item in externalGroup)
            {
                if (item.GroupName.Equals(groupName, System.StringComparison.OrdinalIgnoreCase))
                {
                    isEditExtGroup = true;
                    break;
                }
            }

            if (!isEditExtGroup) // Continue to check new external groups created after launching
            {
                isEditExtGroup = (groupId != 1 && !groupName.Equals("Finance", System.StringComparison.OrdinalIgnoreCase) && !groupName.Equals("Internal", System.StringComparison.OrdinalIgnoreCase));
            }

            if (isEditExtGroup)
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 &&
                    x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageGroup &&
                    x.FuncKey != eFunctions.ManageMenu &&
                    x.FuncKey != eFunctions.ManageUser &&
                    x.FuncKey != eFunctions.Administration &&
                    x.FuncKey != eFunctions.SystemConfiguration &&
                    x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 &&
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
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 &&
                        x.FuncKey != eFunctions.ManageFunction &&
                        x.FuncKey != eFunctions.ManageMenu &&
                        x.FuncKey != eFunctions.SystemConfiguration &&
                        x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncKey != eFunctions.ManageFunction &&
                    x.FuncKey != eFunctions.ManageMenu &&
                    x.FuncKey != eFunctions.SystemConfiguration &&
                    x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 &&
                    x.FuncKey != eFunctions.ExternalGroup);
                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 &&
                    x.FuncKey != eFunctions.ExternalGroup);
            }

            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageGroup, Policy = ePolicy.Update)]
        public ActionResult GrantPermissionPost(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = this.groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId, CurrentContext.Ip4Address, groupId);

            return this.JsonBase(result);
        }
    }
}
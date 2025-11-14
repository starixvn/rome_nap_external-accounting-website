namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using Service.Impl.UserControl;

    public class GroupController : BaseController
    {
        private readonly CachingService _cache;
        private readonly IGroupService _groupService;
        private readonly IFunctionService functionService;

        public GroupController(
            CachingService cache,
            IGroupService groupService,
            IFunctionService functionService)
        {
            _groupService = groupService ?? new GroupService();
            _cache = cache ?? new CachingService();
            this.functionService = functionService ?? new FunctionService();
        }

        public GroupController() : this(null, null, null)
        {
        }   

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GroupList() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public ActionResult AddGroup()
        {
            var groups = new List<SelectListItem> { new SelectListItem { Text = "None", Value = "0" } };
            groups.AddRange(_groupService.GetAllGroup(1).Select(group => new SelectListItem { Text = group.GroupName, Value = group.GroupID.ToString() }).ToList());
            ViewBag.ParentGroups = groups;

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public ActionResult GroupListPost()
        {
            var result = _groupService.GetListGroup().ToList();

            if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup).ToList();
            }
            else
            {
                if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
                {
                    result = result.Where(o => !Group.IsITGroup(o.GroupID)).ToList();
                }
            }

            return JsonBase(new { total = 1, page = 1, records = result.Count, rows = result },
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public ActionResult EditGroup(int id)
        {
            var model = _groupService.GetGroupById(id);

            model.GroupName = HttpUtility.HtmlDecode(model.GroupName);
            model.Description = HttpUtility.HtmlDecode(model.Description);
            model.ParentGroupName = HttpUtility.HtmlDecode(model.ParentGroupName);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public ActionResult GetGroupPolicy(int groupId)
        {
            var model = _groupService.GetAllGroupPolicy(groupId);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string name, string description, int parentId)
        {
            var result = _groupService.CreateGroup(name, description, parentId,  CurrentSession.CustId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPolicy(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = _groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int groupId)
        {
            var result = _groupService.DeleteGroup(groupId, CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int groupId, string groupName, string description)
        {
            var result = _groupService.EditGroup(ExtRequest.GetGroupName(groupName), description, CurrentSession.CustId,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public ActionResult GrantPermission(int groupId, string groupName)
        {
            var funcs = functionService.GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var externalGroup = _groupService.GetExternalGroup();
            var functionList = _groupService.GetAllGroupPolicy(groupId).ToList();

            var allowFunctions = new List<Function>();

            if (!IncludeAllFunctions)
            {
                foreach (var function in functionList)
                {
                    if (funcs.All(f => f.FuncID != function.FuncID))
                    {
                        allowFunctions.Add(function);
                    }
                }

                functionList = allowFunctions;
            }

            ViewBag.GroupID = groupId;
            ViewBag.GroupName = ExtRequest.GetGroupName();
            var isEditExtGroup =
                externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(), StringComparison.OrdinalIgnoreCase))
                ? externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(), StringComparison.OrdinalIgnoreCase))
                : !Group.IsOfficeGroup(ExtRequest.GetGroupName());

            if (isEditExtGroup)
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageGroup
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.ManageUser
                                                           && x.FuncKey != eFunctions.Administration
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu
                    && x.FuncKey != eFunctions.ManageUser
                    && x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (!Group.IsITGroup(groupId))
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                ViewBag.subFunction = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                ViewBag.Function = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                ViewBag.subFunction = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public ActionResult GrantPermissionPost(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = _groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId,
                CurrentContext.Ip4Address, groupId);
            if (result.IsSuccess)
            {
                _cache.RemoveUserCache(CurrentSession.UserId);
            }

            return JsonBase(result);
        }
    }
}
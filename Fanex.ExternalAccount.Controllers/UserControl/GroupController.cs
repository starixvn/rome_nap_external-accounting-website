namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using Service.Impl.Login;
    using Service.Impl.UserControl;
    using Service.Interface.UserControl;

    public class GroupController : BaseController
    {
        private readonly CachingService _cache;
        private readonly IGroupService _groupService;

        public GroupController()
        {
            _groupService = _groupService ?? new GroupService();
            _cache = _cache ?? new CachingService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GroupList() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddGroup() => PartialViewBase();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GroupListPost()
        {
            var result = _groupService.GetListGroup();

            if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }
            else
            {
                if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
                {
                    result = result.Where(o => o.GroupID != 1);
                }
            }

            return JsonBase(new {total = 1, page = 1, records = result.Count(), rows = result},
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditGroup(int id)
        {
            var model = _groupService.GetGroupById(id);

            model.GroupName = HttpUtility.HtmlDecode(model.GroupName);
            model.Description = HttpUtility.HtmlDecode(model.Description);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetGroupPolicy(int groupId)
        {
            var model = _groupService.GetAllGroupPolicy(groupId);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(string name, string description)
        {
          
            var result = _groupService.CreateGroup(name, description, CurrentSession.CustId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditPolicy(int groupId, string oldGroupPolicy, string groupPolicy)
        {
            var result = _groupService.EditGroupPolicy(oldGroupPolicy, groupPolicy, CurrentSession.CustId,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Delete(int groupId)
        {
            var result = _groupService.DeleteGroup(groupId, CurrentSession.CustId);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(int groupId, string groupName, string description)
        {
            var result = _groupService.EditGroup(groupName, description, CurrentSession.CustId,
                CurrentContext.Ip4Address, groupId);

            return JsonBase(result);
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [ExternalAccountAuthenticate(Function = eFunctions.ManageGroup,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GrantPermission(int groupId, string groupName)
        {
            var funcs = new FunctionService().GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var externalGroup = new LoginService().GroupList();
            var functionList = _groupService.GetAllGroupPolicy(groupId);

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

            var isEditExtGroup = false;
            ViewBag.GroupID = groupId;
            ViewBag.GroupName = groupName;

            foreach (var item in externalGroup)
            {
                if (item.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                {
                    isEditExtGroup = true;
                    break;
                }
            }

            if (!isEditExtGroup)
            {
                isEditExtGroup = groupId != 1 && !groupName.Equals("Finance", StringComparison.OrdinalIgnoreCase)
                                 && !groupName.Equals("Internal", StringComparison.OrdinalIgnoreCase);
            }

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
            else if (groupId != 1)
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
        [AntiXSRFFilter]
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
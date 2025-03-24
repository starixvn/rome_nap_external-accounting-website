namespace Fanex.ExternalAccount.Controllers.UserControl
{
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
    using Utilities;

    public class FunctionController : BaseController
    {
        private readonly IFunctionService _funcService = new FunctionService();
        private readonly IGroupService groupService = new GroupService();

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult FunctionList()
        {
            ViewBag.InternalGroupIDs = Group.ITGroupId;

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult FunctionListPost()
        {
            var funcs = _funcService.GetAllFunctions();
            var parents = funcs.Where(o => o.ParentID == 0);
            var funcsOrder = new List<Function>();

            if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                funcs = funcs.Where(o => o.FuncKey != eFunctions.ManageMenu
                                         && o.FuncKey != eFunctions.ManageFunction);

                parents = parents.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
                {
                    funcs = funcs.Where(o => o.FuncKey != eFunctions.ManageMenu
                                             && o.FuncKey != eFunctions.ManageFunction
                                             && o.FuncKey != eFunctions.ManageGroup
                                             && o.FuncKey != eFunctions.ManageUser);

                    parents = parents.Where(o => o.FuncKey != eFunctions.Administration
                                                 && o.FuncKey != eFunctions.SystemConfiguration
                                                 && o.FuncKey != eFunctions.ExternalGroup);
                }
            }

            foreach (var item in parents)
            {
                funcsOrder.Add(item);

                foreach (var child in funcs)
                {
                    if (child.ParentID != 0 && item.FuncID == child.ParentID)
                    {
                        funcsOrder.Add(child);
                    }
                }
            }

            return JsonBase(new { total = 1, page = 1, records = funcsOrder.Count, rows = funcsOrder },
                new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult EditFunction(int id)
        {
            var funcs = _funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = _funcService.GetFunctionPolicy(id);
            var groups = groupService.GetAllGroup();
            var tmp = string.Empty;
            var groupIds = string.Empty;
            var index = 0;

            model.FuncName = HttpUtility.HtmlDecode(model.FuncName);
            model.FuncDesc = HttpUtility.HtmlDecode(model.FuncDesc);
            model.FuncKey = HttpUtility.HtmlDecode(model.FuncKey);

            if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
                {
                    parent = parent.Where(o => o.FuncKey != eFunctions.Administration
                                               && o.FuncKey != eFunctions.SystemConfiguration
                                               && o.FuncKey != eFunctions.ExternalGroup);

                    groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
                }
            }

            model.AssignGroups.ToList().ForEach(o =>
            {
                if (o.HasPermission)
                {
                    tmp += index + ",";
                    groupIds += o.GroupID + ",";
                }

                index++;
            });

            if (tmp.IndexOf(',') != -1)
            {
                tmp = tmp.Substring(0, tmp.Length - 1);
                groupIds = groupIds.Substring(0, groupIds.Length - 1);
            }

            ViewBag.ParentList = parent;
            ViewBag.GroupList = groups;
            ViewBag.OldGroups = tmp;
            ViewBag.OldIDs = groupIds;

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult AddFunction()
        {
            var funcs = _funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = new Function();
            var groups = groupService.GetAllGroup();

            if (!CurrentSession.IsITAdmin && CurrentSession.IsInternalAdmin)
            {
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!CurrentSession.IsITAdmin && !CurrentSession.IsInternalAdmin)
                {
                    parent = parent.Where(o => o.FuncKey != eFunctions.Administration
                                               && o.FuncKey != eFunctions.SystemConfiguration
                                               && o.FuncKey != eFunctions.ExternalGroup);

                    groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
                }
            }

            ViewBag.ParentList = parent;
            ViewBag.GroupList = groups;

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Edit(Function funtion, string oldGroups, string groups)
        {
            var result = _funcService.UpdateFunction(funtion, CurrentSession.CustId,
                CurrentContext.Ip4Address, oldGroups, groups);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult Add(Function function, string groups)
        {
            var result = _funcService.CreateFunction(function, CurrentSession.CustId,
                CurrentContext.Ip4Address, groups);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateStatus(int funcId, bool isTest)
        {
            var result = _funcService.UpdateStatus(funcId, isTest, CurrentSession.CustId, CurrentContext.Ip4Address);

            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ManageFunction,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult PolicyList(int funcId, bool addNew = false)
        {
            var groups = _funcService.GetFunctionPolicy(funcId).AssignGroups.ToList();

            if (!CurrentSession.IsInternalAdmin && !CurrentSession.IsITAdmin)
            {
                groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup).ToList();
            }
            else
            {
                if (funcId == 1 && addNew)
                {
                    groups.ForEach(o => o.HasPermission = Group.IsITGroup(o.GroupID));
                }

                var allChecked = groups.TrueForAll(o => o.HasPermission);

                groups.Insert(
                    0,
                    new FunctionPolicy
                    {
                        GroupID = -1,
                        GroupName = ViewLang.All,
                        HasPermission = allChecked
                    });
            }

            return JsonBase(groups, new JsonSerializerSettings());
        }
    }
}
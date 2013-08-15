// --------------------------------------------------------------------------
// <copyright file="FunctionController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FunctionController : BaseController
    {
        private readonly IFunctionService funcService = new FunctionService();

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Read)]
        public ActionResult FunctionList()
        {
            var internalGroup = new GroupService().GetListGroup()
                .Where(o => o.GroupID == 1).FirstOrDefault();

            var ids = string.Empty;

            ids += internalGroup.GroupID;

            ViewBag.InternalGroupIDs = ids;

            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Read)]
        public ActionResult FunctionListPost()
        {
            var funcs = this.funcService.GetAllFunctions();
            var parents = funcs.Where(o => o.ParentID == 0);
            var funcsOrder = new List<Function>();

            if (!this.CurrentSession.IsITAdmin && this.CurrentSession.IsInternalAdmin)
            {
                // Is Internal admin logged in
                funcs = funcs.Where(o => o.FuncKey != eFunctions.ManageMenu &&
                    o.FuncKey != eFunctions.ManageFunction);

                parents = parents.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else if (!this.CurrentSession.IsITAdmin && !this.CurrentSession.IsInternalAdmin)
            {
                // Is External admin logged in
                funcs = funcs.Where(o => o.FuncKey != eFunctions.ManageMenu &&
                    o.FuncKey != eFunctions.ManageFunction &&
                    o.FuncKey != eFunctions.ManageGroup &&
                    o.FuncKey != eFunctions.ManageUser);

                parents = parents.Where(o => o.FuncKey != eFunctions.Administration &&
                    o.FuncKey != eFunctions.SystemConfiguration &&
                    o.FuncKey != eFunctions.ExternalGroup);
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

            return this.JsonBase(new { total = 1, page = 1, records = funcsOrder.Count(), rows = funcsOrder });
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Read)]
        public ActionResult EditFunction(int id)
        {
            var funcs = this.funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = this.funcService.GetFunctionPolicy(id);
            var groups = new GroupService().GetAllGroup();
            var tmp = string.Empty;
            var groupIds = string.Empty;
            var index = 0;

            model.FuncName = HttpUtility.HtmlDecode(model.FuncName);
            model.FuncDesc = HttpUtility.HtmlDecode(model.FuncDesc);
            model.FuncKey = HttpUtility.HtmlDecode(model.FuncKey);

            if (!this.CurrentSession.IsITAdmin && this.CurrentSession.IsInternalAdmin)
            {
                // Is Internal admin logged in
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else if (!this.CurrentSession.IsITAdmin && !this.CurrentSession.IsInternalAdmin)
            {
                // Is External admin logged in
                parent = parent.Where(o => o.FuncKey != eFunctions.Administration &&
                    o.FuncKey != eFunctions.SystemConfiguration &&
                    o.FuncKey != eFunctions.ExternalGroup);

                groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
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

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Read)]
        public ActionResult AddFunction()
        {
            var funcs = this.funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = new Function();
            var groups = new GroupService().GetAllGroup();

            if (!this.CurrentSession.IsITAdmin && this.CurrentSession.IsInternalAdmin)
            {
                // Is Internal admin logged in
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else if (!this.CurrentSession.IsITAdmin && !this.CurrentSession.IsInternalAdmin)
            {
                // Is External admin logged in
                parent = parent.Where(o => o.FuncKey != eFunctions.Administration &&
                    o.FuncKey != eFunctions.SystemConfiguration &&
                    o.FuncKey != eFunctions.ExternalGroup);

                groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            ViewBag.ParentList = parent;
            ViewBag.GroupList = groups;

            return this.PartialViewBase(model);
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Update)]
        public ActionResult Edit(int funcId, string name, string desc, int parentId, string parent, int order, string oldGroups, string groups)
        {
            var result = this.funcService.UpdateFunction(funcId, name, desc, parentId, parent, order, CurrentSession.CustId, CurrentContext.Ip4Address, oldGroups, groups);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Create)]
        public ActionResult Add(string name, string key, string desc, int parentId, string parent, int order, string groups)
        {
            var funcId = 0;

            var result = this.funcService.CreateFunction(name, key, desc, parentId, parent, order, CurrentSession.CustId, CurrentContext.Ip4Address, groups, out funcId);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Update)]
        public ActionResult UpdateStatus(int funcId, bool isTest)
        {
            var result = this.funcService.UpdateStatus(funcId, isTest, CurrentSession.CustId, CurrentContext.Ip4Address);

            return this.JsonBase(result);
        }

        [Authorization(Function = eFunctions.ManageFunction, Policy = ePolicy.Read)]
        public ActionResult PolicyList(int funcId, bool addNew = false)
        {
            var groups = this.funcService.GetFunctionPolicy(funcId).AssignGroups.ToList();

            if (!CurrentSession.IsInternalAdmin && !CurrentSession.IsITAdmin)
            {
                groups = groups.Where(o => o.GroupName == CurrentSession.CurrencyGroup).ToList();
            }
            else
            {
                if (funcId == 1 && addNew)
                {
                    groups.ForEach(o =>
                    {
                        if (o.GroupID != 1)
                        {
                            o.HasPermission = false;
                        }
                        else
                        {
                            o.HasPermission = true;
                        }
                    });
                }

                var allChecked = groups.TrueForAll(o => o.HasPermission == true);

                groups.Insert(
                    0,
                    new FunctionPolicy
                    {
                        GroupID = -1,
                        GroupName = ViewLang.All,
                        HasPermission = allChecked
                    });
            }

            return this.JsonBase(groups);
        }
    }
}
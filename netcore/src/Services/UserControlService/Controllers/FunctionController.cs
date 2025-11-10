using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (ManageFunction)
    public class FunctionController : ControllerBase
    {
        private readonly IFunctionService _funcService;
        private readonly IGroupService _groupService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public FunctionController(
            IFunctionService funcService,
            IGroupService groupService,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _funcService = funcService;
            _groupService = groupService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("FunctionList")]
        public IActionResult FunctionList()
        {
            // TODO: ViewBag.InternalGroupIDs = Group.ITGroupId;
            var internalGroupIds = 1; // Placeholder
            return Ok(new { internalGroupIds });
        }

        [HttpPost]
        [Route("FunctionListPost")]
        public IActionResult FunctionListPost()
        {
            var funcs = _funcService.GetAllFunctions();
            var parents = funcs.Where(o => o.ParentID == 0);
            var funcsOrder = new List<Function>();

            if (!_currentSession.IsITAdmin && _currentSession.IsInternalAdmin)
            {
                funcs = funcs.Where(o => o.FuncKey != eFunctions.ManageMenu
                                         && o.FuncKey != eFunctions.ManageFunction);

                parents = parents.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!_currentSession.IsITAdmin && !_currentSession.IsInternalAdmin)
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

            return Ok(new { total = 1, page = 1, records = funcsOrder.Count, rows = funcsOrder });
        }

        [HttpGet]
        [Route("EditFunction/{id}")]
        public IActionResult EditFunction(int id)
        {
            var funcs = _funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = _funcService.GetFunctionPolicy(id);
            var groups = _groupService.GetAllGroup();
            var tmp = string.Empty;
            var groupIds = string.Empty;
            var index = 0;

            model.FuncName = System.Net.WebUtility.HtmlDecode(model.FuncName);
            model.FuncDesc = System.Net.WebUtility.HtmlDecode(model.FuncDesc);
            model.FuncKey = System.Net.WebUtility.HtmlDecode(model.FuncKey);

            if (!_currentSession.IsITAdmin && _currentSession.IsInternalAdmin)
            {
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!_currentSession.IsITAdmin && !_currentSession.IsInternalAdmin)
                {
                    parent = parent.Where(o => o.FuncKey != eFunctions.Administration
                                               && o.FuncKey != eFunctions.SystemConfiguration
                                               && o.FuncKey != eFunctions.ExternalGroup);

                    groups = groups.Where(o => o.GroupName == _currentSession.CurrencyGroup);
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

            return Ok(new
            {
                model,
                parentList = parent,
                groupList = groups,
                oldGroups = tmp,
                oldIds = groupIds
            });
        }

        [HttpGet]
        [Route("AddFunction")]
        public IActionResult AddFunction()
        {
            var funcs = _funcService.GetAllFunctions();
            var parent = funcs.Where(o => o.ParentID == 0 && o.FuncKey != eFunctions.ExternalGroup);
            var model = new Function();
            var groups = _groupService.GetAllGroup();

            if (!_currentSession.IsITAdmin && _currentSession.IsInternalAdmin)
            {
                parent = parent.Where(o => o.FuncKey != eFunctions.SystemConfiguration);
            }
            else
            {
                if (!_currentSession.IsITAdmin && !_currentSession.IsInternalAdmin)
                {
                    parent = parent.Where(o => o.FuncKey != eFunctions.Administration
                                               && o.FuncKey != eFunctions.SystemConfiguration
                                               && o.FuncKey != eFunctions.ExternalGroup);

                    groups = groups.Where(o => o.GroupName == _currentSession.CurrencyGroup);
                }
            }

            return Ok(new
            {
                model,
                parentList = parent,
                groupList = groups
            });
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromBody] EditFunctionRequest request)
        {
            var result = _funcService.UpdateFunction(request.Function, _currentSession.UserId,
                _currentContext.Ip4Address, request.OldGroups, request.Groups);

            return Ok(result);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromBody] AddFunctionRequest request)
        {
            var result = _funcService.CreateFunction(request.Function, _currentSession.UserId,
                _currentContext.Ip4Address, request.Groups);

            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var result = _funcService.UpdateStatus(request.FuncId, request.IsTest, _currentSession.UserId, _currentContext.Ip4Address);

            return Ok(result);
        }

        [HttpGet]
        [Route("PolicyList")]
        public IActionResult PolicyList([FromQuery] int funcId, [FromQuery] bool addNew = false)
        {
            var groups = _funcService.GetFunctionPolicy(funcId).AssignGroups.ToList();

            if (!_currentSession.IsInternalAdmin && !_currentSession.IsITAdmin)
            {
                groups = groups.Where(o => o.GroupName == _currentSession.CurrencyGroup).ToList();
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
                        GroupName = "All", // TODO: ViewLang.All
                        HasPermission = allChecked
                    });
            }

            return Ok(groups);
        }
    }

    // TODO: Define proper interfaces and services
    public partial interface IFunctionService
    {
        List<Function> GetAllFunctions();
        FunctionPolicy GetFunctionPolicy(int id);
        object UpdateFunction(Function function, int userId, string ipAddress, string oldGroups, string groups);
        object CreateFunction(Function function, int userId, string ipAddress, string groups);
        object UpdateStatus(int funcId, bool isTest, int userId, string ipAddress);
    }

    public class FunctionService : IFunctionService
    {
        public List<Function> GetAllFunctions()
        {
            // TODO: Implement actual service logic
            return new List<Function>();
        }

        public FunctionPolicy GetFunctionPolicy(int id)
        {
            // TODO: Implement actual service logic
            return new FunctionPolicy { AssignGroups = new List<FunctionPolicy>() };
        }

        public object UpdateFunction(Function function, int userId, string ipAddress, string oldGroups, string groups)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object CreateFunction(Function function, int userId, string ipAddress, string groups)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UpdateStatus(int funcId, bool isTest, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    public partial interface IGroupService
    {
        object GetAllGroup();
    }

    public partial class GroupService : IGroupService
    {
        public object GetAllGroup()
        {
            // TODO: Implement actual service logic
            return new List<object>();
        }
    }

    // TODO: Define proper request/response models
    public class EditFunctionRequest
    {
        public Function Function { get; set; } = new Function();
        public string OldGroups { get; set; } = string.Empty;
        public string Groups { get; set; } = string.Empty;
    }

    public class AddFunctionRequest
    {
        public Function Function { get; set; } = new Function();
        public string Groups { get; set; } = string.Empty;
    }

    public class UpdateStatusRequest
    {
        public int FuncId { get; set; }
        public bool IsTest { get; set; }
    }

    // TODO: Define proper data models
    public class Function
    {
        public int FuncID { get; set; }
        public int ParentID { get; set; }
        public string FuncName { get; set; } = string.Empty;
        public string FuncDesc { get; set; } = string.Empty;
        public string FuncKey { get; set; } = string.Empty;
    }

    public class FunctionPolicy
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
    }

    public static class Group
    {
        public static int ITGroupId => 1;
        public static bool IsITGroup(int groupId) => groupId == ITGroupId;
    }

    public static class eFunctions
    {
        public const string ManageFunction = "ManageFunction";
        public const string ManageMenu = "ManageMenu";
        public const string SystemConfiguration = "SystemConfiguration";
        public const string ManageGroup = "ManageGroup";
        public const string ManageUser = "ManageUser";
        public const string Administration = "Administration";
        public const string ExternalGroup = "ExternalGroup";
    }
}
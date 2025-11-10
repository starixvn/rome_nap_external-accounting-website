using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (ManageGroup)
    public class GroupController : ControllerBase
    {
        private readonly ICachingService _cache;
        private readonly IGroupService _groupService;
        private readonly IFunctionService _functionService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public GroupController(
            ICachingService cache,
            IGroupService groupService,
            IFunctionService functionService,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _cache = cache;
            _groupService = groupService;
            _functionService = functionService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("GroupList")]
        public IActionResult GroupList()
        {
            return Ok(new { message = "Group list view" });
        }

        [HttpGet]
        [Route("AddGroup")]
        public IActionResult AddGroup()
        {
            var groups = new List<SelectListItem> { new SelectListItem { Text = "None", Value = "0" } };
            groups.AddRange(_groupService.GetAllGroup(1).Select(group => new SelectListItem { Text = group.GroupName, Value = group.GroupID.ToString() }).ToList());

            return Ok(new { parentGroups = groups });
        }

        [HttpPost]
        [Route("GroupListPost")]
        public IActionResult GroupListPost()
        {
            var result = _groupService.GetListGroup().ToList();

            if (!_currentSession.IsITAdmin && !_currentSession.IsInternalAdmin)
            {
                result = result.Where(o => o.GroupName == _currentSession.CurrencyGroup).ToList();
            }
            else
            {
                if (!_currentSession.IsITAdmin && _currentSession.IsInternalAdmin)
                {
                    result = result.Where(o => !Group.IsITGroup(o.GroupID)).ToList();
                }
            }

            return Ok(new { total = 1, page = 1, records = result.Count, rows = result });
        }

        [HttpGet]
        [Route("EditGroup/{id}")]
        public IActionResult EditGroup(int id)
        {
            var model = _groupService.GetGroupById(id);

            model.GroupName = System.Net.WebUtility.HtmlDecode(model.GroupName);
            model.Description = System.Net.WebUtility.HtmlDecode(model.Description);
            model.ParentGroupName = System.Net.WebUtility.HtmlDecode(model.ParentGroupName);

            return Ok(model);
        }

        [HttpGet]
        [Route("GetGroupPolicy/{groupId}")]
        public IActionResult GetGroupPolicy(int groupId)
        {
            var model = _groupService.GetAllGroupPolicy(groupId);
            return Ok(model);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromBody] AddGroupRequest request)
        {
            var result = _groupService.CreateGroup(request.Name, request.Description, request.ParentId, _currentSession.UserId, _currentContext.Ip4Address);
            return Ok(result);
        }

        [HttpPost]
        [Route("EditPolicy")]
        public IActionResult EditPolicy([FromBody] EditPolicyRequest request)
        {
            var result = _groupService.EditGroupPolicy(request.OldGroupPolicy, request.GroupPolicy, _currentSession.UserId,
                _currentContext.Ip4Address, request.GroupId);

            return Ok(result);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete([FromBody] DeleteGroupRequest request)
        {
            var result = _groupService.DeleteGroup(request.GroupId, _currentSession.UserId);
            return Ok(result);
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromBody] EditGroupRequest request)
        {
            var result = _groupService.EditGroup(ExtRequest.GetGroupName(request.GroupName), request.Description, _currentSession.UserId,
                _currentContext.Ip4Address, request.GroupId);

            return Ok(result);
        }

        [HttpGet]
        [Route("GrantPermission")]
        public IActionResult GrantPermission([FromQuery] int groupId, [FromQuery] string groupName)
        {
            var funcs = _functionService.GetAllFunctions().ToList();
            funcs = funcs.Where(f => f.IsTest).ToList();
            var externalGroup = _groupService.GetExternalGroup();
            var functionList = _groupService.GetAllGroupPolicy(groupId).ToList();

            var allowFunctions = new List<Function>();

            if (!GroupControllerExtensions.IncludeAllFunctions)
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

            var groupIdResult = groupId;
            var groupNameResult = ExtRequest.GetGroupName();
            var isEditExtGroup =
                externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(), StringComparison.OrdinalIgnoreCase))
                ? externalGroup.Any(i => i.GroupName.Equals(ExtRequest.GetGroupName(), StringComparison.OrdinalIgnoreCase))
                : !Group.IsOfficeGroup(ExtRequest.GetGroupName());

            object viewBagFunction = null!;
            object viewBagSubFunction = null!;

            if (isEditExtGroup)
            {
                viewBagFunction = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageGroup
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.ManageUser
                                                           && x.FuncKey != eFunctions.Administration
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                viewBagSubFunction = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageGroup && x.FuncKey != eFunctions.ManageMenu
                    && x.FuncKey != eFunctions.ManageUser
                    && x.FuncKey != eFunctions.Administration && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else if (!Group.IsITGroup(groupId))
            {
                viewBagFunction = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ManageFunction
                                                           && x.FuncKey != eFunctions.ManageMenu
                                                           && x.FuncKey != eFunctions.SystemConfiguration
                                                           && x.FuncKey != eFunctions.ExternalGroup);

                viewBagSubFunction = functionList.Where(x =>
                    x.ParentID != 0 && x.FuncKey != eFunctions.ManageFunction
                    && x.FuncKey != eFunctions.ManageMenu && x.FuncKey != eFunctions.SystemConfiguration
                    && x.FuncKey != eFunctions.ExternalGroup);
            }
            else
            {
                viewBagFunction = functionList.Where(x => x.ParentID == 0 && x.FuncKey != eFunctions.ExternalGroup);
                viewBagSubFunction = functionList.Where(x => x.ParentID != 0 && x.FuncKey != eFunctions.ExternalGroup);
            }

            return Ok(new
            {
                groupId = groupIdResult,
                groupName = groupNameResult,
                functions = viewBagFunction,
                subFunctions = viewBagSubFunction
            });
        }

        [HttpPost]
        [Route("GrantPermissionPost")]
        public IActionResult GrantPermissionPost([FromBody] GrantPermissionRequest request)
        {
            var result = _groupService.EditGroupPolicy(request.OldGroupPolicy, request.GroupPolicy, _currentSession.UserId,
                _currentContext.Ip4Address, request.GroupId);
            if (result.IsSuccess)
            {
                _cache.RemoveUserCache(_currentSession.UserId);
            }

            return Ok(result);
        }
    }

    // TODO: Define proper interfaces and services
    public interface ICachingService
    {
        void RemoveUserCache(int userId);
    }

    public class CachingService : ICachingService
    {
        public void RemoveUserCache(int userId)
        {
            // TODO: Implement actual cache removal logic
        }
    }

    // TODO: Define proper request/response models
    public class AddGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ParentId { get; set; }
    }

    public class EditPolicyRequest
    {
        public int GroupId { get; set; }
        public string OldGroupPolicy { get; set; } = string.Empty;
        public string GroupPolicy { get; set; } = string.Empty;
    }

    public class DeleteGroupRequest
    {
        public int GroupId { get; set; }
    }

    public class EditGroupRequest
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class GrantPermissionRequest
    {
        public int GroupId { get; set; }
        public string OldGroupPolicy { get; set; } = string.Empty;
        public string GroupPolicy { get; set; } = string.Empty;
    }

    // TODO: Extend IGroupService with additional methods
    public partial interface IGroupService
    {
        object GetListGroup();
        object GetGroupById(int id);
        object GetAllGroupPolicy(int groupId);
        object CreateGroup(string name, string description, int parentId, int userId, string ipAddress);
        object EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int userId, string ipAddress, int groupId);
        object DeleteGroup(int groupId, int userId);
        object EditGroup(string groupName, string description, int userId, string ipAddress, int groupId);
        object GetExternalGroup();
    }

    // TODO: Extend IGroupService implementation
    public partial class GroupService
    {
        public object GetListGroup()
        {
            // TODO: Implement actual service logic
            return new List<object>();
        }

        public object GetGroupById(int id)
        {
            // TODO: Implement actual service logic
            return new { GroupName = "Test Group", Description = "Test", ParentGroupName = "Parent" };
        }

        public object GetAllGroupPolicy(int groupId)
        {
            // TODO: Implement actual service logic
            return new List<Function>();
        }

        public object CreateGroup(string name, string description, int parentId, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object EditGroupPolicy(string oldGroupPolicy, string groupPolicy, int userId, string ipAddress, int groupId)
        {
            // TODO: Implement actual service logic
            return new { IsSuccess = true };
        }

        public object DeleteGroup(int groupId, int userId)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object EditGroup(string groupName, string description, int userId, string ipAddress, int groupId)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object GetExternalGroup()
        {
            // TODO: Implement actual service logic
            return new List<object>();
        }
    }

    // TODO: Extend IFunctionService with additional methods
    public partial interface IFunctionService
    {
        // Add any missing methods if needed
    }

    // TODO: Define additional classes
    public class SelectListItem
    {
        public string Text { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public static class ExtRequest
    {
        public static string GetGroupName(string groupName = null)
        {
            // TODO: Implement actual logic
            return groupName ?? "default";
        }
    }

    public static class GroupExtensions
    {
        public static bool IsOfficeGroup(string groupName)
        {
            // TODO: Implement actual logic
            return false;
        }
    }

    public static class GroupControllerExtensions
    {
        public static bool IncludeAllFunctions => true; // TODO: Implement actual logic
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (GrantPermission)
    public class PermissionController : ControllerBase
    {
        private readonly IPolicyService _policyService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public PermissionController(
            IPolicyService policyService,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _policyService = policyService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index([FromQuery] int? funcId)
        {
            var hierarchicalFunctions = _policyService.GetHierarchicalFunctions();
            return Ok(new { funcId, hierarchicalFunctions });
        }

        [HttpGet]
        [Route("GetGroupPolicies")]
        public IActionResult GetGroupPolicies([FromQuery] int funcId)
        {
            var groupPolicies = _policyService.GetGroupPolicies(funcId);

            if (!_currentSession.IsITAdmin)
            {
                groupPolicies = _currentSession.IsInternalAdmin
                    ? groupPolicies.Where(o => !Group.IsITGroup(o.GroupId))
                    : groupPolicies.Where(o => o.GroupName == _currentSession.CurrencyGroup);
            }

            return Ok(groupPolicies);
        }

        [HttpGet]
        [Route("GetUserPolicies")]
        public IActionResult GetUserPolicies([FromQuery] int funcId)
        {
            var userPolicies = _policyService.GetUserPolicies(funcId);

            if (!_currentSession.IsITAdmin)
            {
                userPolicies = _currentSession.IsInternalAdmin
                    ? userPolicies.Where(o => !Group.IsITGroup(o.GroupId))
                    : userPolicies.Where(o => o.GroupName == _currentSession.CurrencyGroup);
            }

            return Ok(userPolicies);
        }

        [HttpPost]
        [Route("UpdateGroupPolicy")]
        public IActionResult UpdateGroupPolicy([FromBody] UpdateGroupPolicyRequest request)
        {
            var result = _policyService.UpdateMultipleGroupPolicies(request.FuncId, request.Policies, _currentSession.UserId, _currentContext.Ip4Address);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateUserPolicy")]
        public IActionResult UpdateUserPolicy([FromBody] UpdateUserPolicyRequest request)
        {
            var result = _policyService.UpdateMultipleUserPolicies(request.FuncId, request.Policies, _currentSession.UserId, _currentContext.Ip4Address);
            return Ok(result);
        }
    }

    // TODO: Define proper interfaces and services
    public interface IPolicyService
    {
        object GetHierarchicalFunctions();
        object GetGroupPolicies(int funcId);
        object GetUserPolicies(int funcId);
        object UpdateMultipleGroupPolicies(int funcId, string policies, int userId, string ipAddress);
        object UpdateMultipleUserPolicies(int funcId, string policies, int userId, string ipAddress);
    }

    public class PolicyService : IPolicyService
    {
        public object GetHierarchicalFunctions()
        {
            // TODO: Implement actual service logic
            return new { functions = new[] { new { id = 1, name = "Function 1" } } };
        }

        public object GetGroupPolicies(int funcId)
        {
            // TODO: Implement actual service logic
            return new[] { new { groupId = 1, groupName = "Group 1", funcId = funcId } };
        }

        public object GetUserPolicies(int funcId)
        {
            // TODO: Implement actual service logic
            return new[] { new { userId = 1, userName = "User 1", funcId = funcId } };
        }

        public object UpdateMultipleGroupPolicies(int funcId, string policies, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UpdateMultipleUserPolicies(int funcId, string policies, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    // TODO: Define proper request models
    public class UpdateGroupPolicyRequest
    {
        public int FuncId { get; set; }
        public string Policies { get; set; } = string.Empty;
    }

    public class UpdateUserPolicyRequest
    {
        public int FuncId { get; set; }
        public string Policies { get; set; } = string.Empty;
    }
}
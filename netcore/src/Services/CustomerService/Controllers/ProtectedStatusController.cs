using Fanex.ExternalAccount.Controllers.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (ProtectCustomer)
    public class ProtectedStatusController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;
        private readonly IAuthorizationService _authorizationService;

        public ProtectedStatusController(
            ICustomerService customerService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IAuthorizationService authorizationService)
        {
            _customerService = customerService;
            _currentSession = currentSession;
            _currentContext = currentContext;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("OpenMultiple")]
        public IActionResult OpenMultiple([FromQuery] string groupName, [FromQuery] string custIds, [FromQuery] byte roleId)
        {
            var result = _customerService.GetMultipleProtectedStatus(groupName, custIds, roleId, _currentSession.UserId);
            return Ok(result);
        }

        [HttpPost]
        [Route("EditMultiple")]
        public IActionResult EditMultiple([FromQuery] string groupName, [FromBody] CustomerProtectedStatus[] protectedStatus)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new System.ArgumentNullException(nameof(groupName));
            }

            var custIds = protectedStatus.Select(item => item.CustId);

            // TODO: Implement proper authorization check
            // if (!AuthorizationService.IsBelong(groupName, custIds))
            // {
            //     return StatusCode((int)HttpStatusCode.Forbidden, new { message = "Don't have permission" });
            // }

            var results = _customerService.UpdateMultipleProtectedStatus(
                protectedStatus,
                _currentSession.UserId,
                _currentContext.Ip4Address);

            return Ok(results);
        }
    }

    // TODO: Define proper interfaces and models
    public partial interface ICustomerService
    {
        object GetMultipleProtectedStatus(string groupName, string custIds, byte roleId, int userId);
        object UpdateMultipleProtectedStatus(CustomerProtectedStatus[] protectedStatus, int userId, string ipAddress);
    }

    public partial class CustomerService : ICustomerService
    {
        public object GetMultipleProtectedStatus(string groupName, string custIds, byte roleId, int userId)
        {
            // TODO: Implement actual service logic
            return new[] { new { custId = 1, status = "protected" } };
        }

        public object UpdateMultipleProtectedStatus(CustomerProtectedStatus[] protectedStatus, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true, count = protectedStatus.Length };
        }
    }

    public interface IAuthorizationService
    {
        // TODO: Implement authorization logic
    }

    public class AuthorizationService : IAuthorizationService
    {
        // TODO: Implement authorization logic
    }

    // TODO: Define proper models
    public class CustomerProtectedStatus
    {
        public int CustId { get; set; }
        // TODO: Add other properties as needed
    }

    public class Result
    {
        public Result(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public int ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}

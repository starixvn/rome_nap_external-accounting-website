using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.UserControl
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (SubGroup)
    public class SubGroupController : ControllerBase
    {
        private readonly ISubGroupService _subGroupService;
        private readonly IGroupService _groupService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public SubGroupController(
            ISubGroupService subGroupService,
            IGroupService groupService,
            ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            _subGroupService = subGroupService;
            _groupService = groupService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("SubGroupList")]
        public IActionResult SubGroupList()
        {
            return Ok(new { message = "SubGroup list view" });
        }

        [HttpPost]
        [Route("SubGroupPost")]
        public IActionResult SubGroupPost([FromBody] SubGroupPostRequest request)
        {
            if (!_groupService.CheckIsMainGroup(request.GroupId))
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new Result((int)HttpStatusCode.Unauthorized));
            }

            var groups = _groupService.GetGroupWithSubGroups(request.GroupId);

            if (!groups.Any())
            {
                return StatusCode((int)HttpStatusCode.NoContent, new Result((int)HttpStatusCode.NoContent));
            }

            return Ok(groups.Where(group => group.ParentGroupId != 0));
        }

        [HttpPost]
        [Route("SearchSubGroupCustomer")]
        public IActionResult SearchSubGroupCustomer([FromBody] SearchSubGroupCustomerRequest request)
        {
            var model = _subGroupService.GetListCustomerSubGroup(request.GroupName, request.SubGroupName, request.Username, request.PageSize, request.PageIndex);
            return Ok(model);
        }

        [HttpPost]
        [Route("SubGroupUpdate")]
        public IActionResult SubGroupUpdate([FromBody] SubGroupUpdateRequest request)
        {
            var result = _subGroupService.UpdateCustomerSubGroup(request.SubGroupName, request.CustIds, request.IsAssigned, _currentSession.UserId, _currentContext.Ip4Address);
            return Ok(result);
        }

        [HttpGet]
        [Route("ChangeSubGroupAdminPassword")]
        public IActionResult ChangeSubGroupAdminPassword([FromQuery] string groupName)
        {
            var admins = _subGroupService.GetListAdminSubGroup(groupName).Select(admin => new SelectListItem { Text = admin.Username, Value = admin.CustId.ToString() }).ToList();
            var sCode = "placeholder_captcha"; // TODO: Encryptor.RandomCaptcha

            return Ok(new { admins, groupName, sCode });
        }

        [HttpPost]
        [Route("UpdateAdminPassword")]
        public IActionResult UpdateAdminPassword([FromBody] UpdateAdminPasswordRequest request)
        {
            // TODO: var rawPassword = Encryptor.Decrypt(request.Password, Encryptor.RandomCaptcha);
            var rawPassword = request.Password; // Placeholder - decryption needed
            // TODO: var result = _subGroupService.UpdateSubGroupAdminPassword(request.AdminId, Encryption.CfsEnCode(rawPassword), _currentSession.UserId, _currentContext.Ip4Address);
            var result = new { success = true }; // Placeholder
            return Ok(result);
        }
    }

    // TODO: Define proper interfaces and services
    public interface ISubGroupService
    {
        object GetListCustomerSubGroup(string groupName, string subGroupName, string username, int pageSize, int pageIndex);
        object UpdateCustomerSubGroup(string subGroupName, object custIds, bool isAssigned, int userId, string ipAddress);
        object GetListAdminSubGroup(string groupName);
        object UpdateSubGroupAdminPassword(int adminId, string encryptedPassword, int userId, string ipAddress);
    }

    public class SubGroupService : ISubGroupService
    {
        public object GetListCustomerSubGroup(string groupName, string subGroupName, string username, int pageSize, int pageIndex)
        {
            // TODO: Implement actual service logic
            return new { customers = new[] { new { username = "user1", groupName = groupName } } };
        }

        public object UpdateCustomerSubGroup(string subGroupName, object custIds, bool isAssigned, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object GetListAdminSubGroup(string groupName)
        {
            // TODO: Implement actual service logic
            return new[] { new { custId = 1, username = "admin1" } };
        }

        public object UpdateSubGroupAdminPassword(int adminId, string encryptedPassword, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    // TODO: Extend IGroupService with additional methods
    public partial interface IGroupService
    {
        bool CheckIsMainGroup(int groupId);
        object GetGroupWithSubGroups(int groupId);
    }

    // TODO: Extend IGroupService implementation
    public partial class GroupService
    {
        public bool CheckIsMainGroup(int groupId)
        {
            // TODO: Implement actual service logic
            return true;
        }

        public object GetGroupWithSubGroups(int groupId)
        {
            // TODO: Implement actual service logic
            return new[] { new { groupId = groupId, parentGroupId = 0 } };
        }
    }

    // TODO: Define proper request/response models
    public class SubGroupPostRequest
    {
        public int GroupId { get; set; }
    }

    public class SearchSubGroupCustomerRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public string SubGroupName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class SubGroupUpdateRequest
    {
        public string SubGroupName { get; set; } = string.Empty;
        public object CustIds { get; set; } = null!;
        public bool IsAssigned { get; set; }
    }

    public class UpdateAdminPasswordRequest
    {
        public int AdminId { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    // TODO: Define proper data models
    public class Result
    {
        public Result(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; }
    }

}
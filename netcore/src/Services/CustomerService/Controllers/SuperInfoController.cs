using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (EditSuperInfo Host)
    public class SuperInfoController : ControllerBase
    {
        private readonly IEditSuperListService _superListService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public SuperInfoController(IEditSuperListService superListService, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            _superListService = superListService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            // TODO: Generate proper captcha code
            var sCode = "placeholder_captcha";

            return Ok(new { SCode = sCode });
        }

        [HttpPost]
        [Route("CheckMultipleSupers")]
        public IActionResult CheckMultipleSupers([FromQuery] string destUsers, [FromQuery] string groupName)
        {
            var result = _superListService.CheckMultipleSupers(destUsers, groupName, _currentSession.UserId);
            return Ok(new
            {
                result.ErrorCode,
                result.ErrorMessage,
                SuperIds = result.ResultData
            });
        }

        [HttpPost]
        [Route("UpdateMultipleSupers")]
        public IActionResult UpdateMultipleSupers([FromBody] UpdateMultipleSuperRequest request)
        {
            // TODO: Implement password decryption
            // request.Password = Encryptor.Decrypt(request.Password, captchaCode);
            var result = _superListService.UpdateMultipleSuperRequest(request, _currentSession.UserId, _currentContext.Ip4Address);
            return Ok(result);
        }
    }

    // TODO: Define proper interfaces and models
    public interface IEditSuperListService
    {
        (int ErrorCode, string ErrorMessage, object ResultData) CheckMultipleSupers(string destUsers, string groupName, int userId);
        object UpdateMultipleSuperRequest(UpdateMultipleSuperRequest request, int userId, string ipAddress);
    }

    public class EditSuperListService : IEditSuperListService
    {
        public (int ErrorCode, string ErrorMessage, object ResultData) CheckMultipleSupers(string destUsers, string groupName, int userId)
        {
            // TODO: Implement actual service logic
            return (0, "Success", new[] { 1, 2, 3 });
        }

        public object UpdateMultipleSuperRequest(UpdateMultipleSuperRequest request, int userId, string ipAddress)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    public interface ICurrentContext
    {
        string Ip4Address { get; }
    }

    public class CurrentContext : ICurrentContext
    {
        public string Ip4Address => "127.0.0.1"; // TODO: Implement proper IP detection
    }

    // TODO: Define proper request model
    public class UpdateMultipleSuperRequest
    {
        public string Password { get; set; } = string.Empty;
        // TODO: Add other properties as needed
    }
}

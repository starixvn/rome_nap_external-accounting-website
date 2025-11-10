using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Fanex.ExternalAccount.Controllers.Home
{
    [ApiController]
    [Route("api/[controller]")]
    public class NetworkSpeedController : ControllerBase
    {
        private readonly IEnumerable<int> CalculatingNWSpeedGroupIds;
        private readonly IEnumerable<string> CalculatingNWSpeedAdminNames;
        private readonly ICurrentSession _currentSession;
        private readonly ILogger<NetworkSpeedController> _logger;
        private readonly IConfiguration _configuration;

        public NetworkSpeedController(ICurrentSession currentSession, ILogger<NetworkSpeedController> logger, IConfiguration configuration)
        {
            _currentSession = currentSession;
            _logger = logger;
            _configuration = configuration;

            CalculatingNWSpeedGroupIds = (string.IsNullOrEmpty(_configuration["TrackConnectionSpeedGroupIds"])
                ? Enumerable.Empty<int>()
                : _configuration["TrackConnectionSpeedGroupIds"]
                    .Split(',')
                    .Select(groupIdString => ConvertStringToInt(groupIdString)))
                .Where(groupId => groupId != 0);

            CalculatingNWSpeedAdminNames = string.IsNullOrEmpty(_configuration["CalculatingNWSpeedAdminNames"])
                ? Enumerable.Empty<string>()
                : _configuration["CalculatingNWSpeedAdminNames"]
                    .Split(',')
                    .Select(adminNames => adminNames.Trim());
        }

        [HttpGet]
        [Route("Index")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Index()
        {
            if (_currentSession.IsLogin && User.Identity?.IsAuthenticated == true)
            {
                if (!IsNeedToCalculateUserConnectionSpeed())
                {
                    return NoContent();
                }

                return Ok(new { message = "Network speed tracking enabled" });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("SaveResult")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult SaveResult([FromQuery] float speedKbps)
        {
            if (_currentSession.IsLogin
                && User.Identity?.IsAuthenticated == true
                && speedKbps > 0
                && IsNeedToCalculateUserConnectionSpeed())
            {
                LogUserInformation(speedKbps);
            }

            return NoContent();
        }

        private bool IsNeedToCalculateUserConnectionSpeed()
        {
            if (CalculatingNWSpeedGroupIds.Any(groupId => groupId == -1 || _currentSession.GroupId == groupId))
            {
                return true;
            }

            if (CalculatingNWSpeedAdminNames.Any(adminUsername => adminUsername.Trim().Equals("*") || _currentSession.UserName.Equals(adminUsername.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private void LogUserInformation(float speedKbps)
        {
            // TODO: Implement proper request info extraction
            var request = HttpContext.Request;
            var message = string.Join(
                "\r\n",
                "\r\nUser connection information:",
                $"Line: {_currentSession.CurrentLine}",
                $"Connection Speed: {speedKbps} kbps",
                $"Browser: {Request.Headers["User-Agent"].ToString()}",
                $"Domain: {request.Host}",
                $"User Id: {_currentSession.UserId}",
                $"Username: {_currentSession.UserName}",
                $"Group Id: {_currentSession.GroupId}",
                $"Group Name: {_currentSession.CurrencyGroup}");

            _logger.LogInformation(message);
        }

        private int ConvertStringToInt(string strInput)
            => strInput.Trim().Equals("*") ? -1 : int.TryParse(strInput, out var result) ? result : 0;
    }
}
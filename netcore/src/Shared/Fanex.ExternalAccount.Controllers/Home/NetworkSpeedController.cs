namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Common;
    using Core.Base;
    using Core.Helpers;
    using Core.Context;
    using Core.Authorization;

    public class NetworkSpeedController : BaseController
    {
        private readonly IEnumerable<int> CalculatingNWSpeedGroupIds;
        private readonly IEnumerable<string> CalculatingNWSpeedAdminNames;
        private readonly IConfiguration _configuration;

        public NetworkSpeedController(ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _configuration = configuration;
            
            var trackConnectionSpeedGroupIds = _configuration["TrackConnectionSpeedGroupIds"];
            CalculatingNWSpeedGroupIds = (string.IsNullOrEmpty(trackConnectionSpeedGroupIds)
                ? Enumerable.Empty<int>()
                : trackConnectionSpeedGroupIds
                    .Split(',')
                    .Select(groupIdString => ConvertStringToInt(groupIdString)))
                .Where(groupId => groupId != 0);

            var calculatingNWSpeedAdminNames = _configuration["CalculatingNWSpeedAdminNames"];
            CalculatingNWSpeedAdminNames = string.IsNullOrEmpty(calculatingNWSpeedAdminNames)
                ? Enumerable.Empty<string>()
                : calculatingNWSpeedAdminNames
                    .Split(',')
                    .Select(adminNames => adminNames.Trim());
        }

        public IActionResult Index()
        {
            if (CurrentSession.IsLogin
                && User.Identity?.IsAuthenticated == true)
            {
                if (!IsNeedToCalculateUserConnectionSpeed())
                {
                    return new EmptyResult();
                }

                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult SaveResult(float speedKbps)
        {
            if (CurrentSession.IsLogin
                && User.Identity?.IsAuthenticated == true
                && speedKbps > 0
                && IsNeedToCalculateUserConnectionSpeed())
            {
                LogUserInformation(speedKbps);
            }

            return new EmptyResult();
        }

        private bool IsNeedToCalculateUserConnectionSpeed()
        {
            if (CalculatingNWSpeedGroupIds.Any(groupId => groupId == -1 || CurrentSession.GroupId == groupId))
            {
                return true;
            }

            if (CalculatingNWSpeedAdminNames.Any(adminUsername => adminUsername.Trim().Equals("*") || CurrentSession.UserName.Equals(adminUsername.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private void LogUserInformation(float speedKbps)
        {
            var request = RequestHelper.GetRequestInfo();
            var message = string.Join(
                "\r\n",
                "\r\nUser connection information:",
                $"Line: {new Core.Context.CurrentSession().CurrentLine}",
                $"Connection Speed: {speedKbps} kbps",
                $"Browser: {request.Browser}",
                $"Platform: {request.Platform}",
                $"Domain: {request.DomainName}",
                $"NetSessionId: {request.NetSessionId}",
                $"User Id: {CurrentSession.UserId}",
                $"Username: {CurrentSession.UserName}",
                $"Group Id: {CurrentSession.GroupId}",
                $"Group Name: {CurrentSession.CurrencyGroup}");

            Logging.LogService.Info(message);
        }

        private int ConvertStringToInt(string strInput)
            => strInput.Trim().Equals("*") ? -1 : Converter.ToInt(strInput);
    }
}
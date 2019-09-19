namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using Common;
    using Core.Base;
    using Core.Helpers;

    public class NetworkSpeedController : BaseController
    {
        private readonly IEnumerable<int> CalculatingNWSpeedGroupIds = string.IsNullOrEmpty(ConfigurationManager.AppSettings["TrackConnectionSpeedGroupIds"])
            ? Enumerable.Empty<int>()
            : ConfigurationManager.AppSettings["TrackConnectionSpeedGroupIds"]
                .Split(',')
                .Select(groupIdString => Converter.ToInt(groupIdString));
        private readonly IEnumerable<string> IngoredCalculatingNWSpeedAdminNames = string.IsNullOrEmpty(ConfigurationManager.AppSettings["IngoredCalculatingNWSpeedAdminNames"])
            ? Enumerable.Empty<string>()
            : ConfigurationManager.AppSettings["IngoredCalculatingNWSpeedAdminNames"].Split(',');

        private const string LoggedNWSpeedAdminName = "starix";
        private const int ITGroupId = 1;

        public ActionResult Index()
        {
            if (CurrentSession.IsLogin
                && User.Identity.IsAuthenticated)
            {
                if(!IsNeedToCalculateUserConnectionSpeed())
                {
                    return new EmptyResult();
                }

                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        public ActionResult SaveResult(float speedKbps)
        {
            if (CurrentSession.IsLogin
                && User.Identity.IsAuthenticated
                && speedKbps > 0 
                && CalculatingNWSpeedGroupIds.Any())
            {
                LogUserInformation(speedKbps);
            }

            return new EmptyResult();
        }

        private bool IsNeedToCalculateUserConnectionSpeed()
        {
            var adminUsername = CurrentSession.UserName;
            foreach (var groupId in CalculatingNWSpeedGroupIds)
            {
                if (groupId == 0)
                {
                    return true;
                }

                if (groupId > 0 && CurrentSession.GroupId == groupId)
                {
                    if (groupId == ITGroupId
                        && !adminUsername.StartsWith(LoggedNWSpeedAdminName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    if(IngoredCalculatingNWSpeedAdminNames
                        .Any(adminName => string.Compare(adminName, adminUsername, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        continue;
                    }

                    return true;
                }
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
    }
}
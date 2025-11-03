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
        private readonly IEnumerable<int> CalculatingNWSpeedGroupIds;
        private readonly IEnumerable<string> CalculatingNWSpeedAdminNames;

        public NetworkSpeedController()
        {
            CalculatingNWSpeedGroupIds = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TrackConnectionSpeedGroupIds"])
                ? Enumerable.Empty<int>()
                : ConfigurationManager.AppSettings["TrackConnectionSpeedGroupIds"]
                    .Split(',')
                    .Select(groupIdString => ConvertStringToInt(groupIdString)))
                .Where(groupId => groupId != 0);

            CalculatingNWSpeedAdminNames = string.IsNullOrEmpty(ConfigurationManager.AppSettings["CalculatingNWSpeedAdminNames"])
                ? Enumerable.Empty<string>()
                : ConfigurationManager.AppSettings["CalculatingNWSpeedAdminNames"]
                    .Split(',')
                    .Select(adminNames => adminNames.Trim());
        }

        public ActionResult Index()
        {
            if (CurrentSession.IsLogin
                && User.Identity.IsAuthenticated)
            {
                if (!IsNeedToCalculateUserConnectionSpeed())
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
namespace Fanex.ExternalAccount.UI.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return true;
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            return true ;
        }

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext)
        {
            return false;
        }

        public override SettingContext FetchSettingContext()
        {
            var currentSession = new Fanex.ExternalAccount.Core.Context.CurrentSession();
            var settingContext = new SettingContext();

            settingContext.ActorId = (int)currentSession.UserId;
            settingContext.ActorName = (string)currentSession.UserName;
            settingContext.ActorLevel = 5;
            settingContext.SubAccountId = 0;
            settingContext.Language = "en-US";
            settingContext.Domain = HttpContext.Current.Request.Url.Host;
            settingContext.IP = GetClientIp();

            return settingContext;
        }

        private string GetClientIp()
        {
            var forwardHeader = GetForwardHeader();

            if (!string.IsNullOrEmpty(forwardHeader))
            {
                return forwardHeader;
            }

            return GetInternalAddress();
        }

        private string GetForwardHeader()
        {
            var httpRequest = HttpContext.Current.Request;
            var forwardHeader = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (forwardHeader != null)
            {
                return forwardHeader.Split(',').First().Trim();
            }

            return string.Empty;
        }

        private string GetInternalAddress()
        {
            var httpRequest = HttpContext.Current.Request;
            string userHostAddress = httpRequest.UserHostAddress;
            var internalIPAddress = GetInternalIPAddress(userHostAddress);

            if (internalIPAddress == null)
            {
                var hostName = Dns.GetHostName();

                internalIPAddress = GetInternalIPAddress(hostName);
            }

            return internalIPAddress != null ? internalIPAddress.ToString() : string.Empty;
        }

        private IPAddress GetInternalIPAddress(string hostAddress)
        {
            foreach (var itemIPA in Dns.GetHostAddresses(hostAddress))
            {
                if (itemIPA.AddressFamily.ToString().Equals("InterNetwork", StringComparison.OrdinalIgnoreCase))
                {
                    return itemIPA;
                }
            }

            return null;
        }
    }
}
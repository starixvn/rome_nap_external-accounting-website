namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;
    using Newtonsoft.Json;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return this.AllowRead();
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated(permissionContext);
        }

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated(permissionContext);
        }

        public override SettingContext FetchSettingContext()
        {
            var currentSession = new Fanex.ExternalAccount.Core.Context.CurrentSession();
            var settingContext = new SettingContext();

            settingContext.ActorId = currentSession.UserId;
            settingContext.ActorName = currentSession.UserName;
            settingContext.ActorLevel = 5;
            settingContext.SubAccountId = 0;
            settingContext.Language = CultureInfo.CurrentCulture.Name;
            settingContext.Domain = HttpContext.Current.Request.Url.Host;
            settingContext.IP = CurrentContext.Instance.Ip4Address;         

            return settingContext;
        }        

        private bool AllowRead()
        {
            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.Sportsbook2Setting);
        }

        private bool AllowUpdated(PermissionContext permissionContext)
        {
            bool isSuper = this.CheckIsSuperLevel(permissionContext);
            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            return isSuper ? false : _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.Sportsbook2Setting);
        }

        private bool CheckIsSuperLevel(PermissionContext permissionContext)
        {
            var trackingValue = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("setting").AttemptedValue;
            if (!string.IsNullOrWhiteSpace(trackingValue))
            {
                var pattern = "(\"customerLevel\":{\"value\")([^\r\n][0-9]+)";
                Match regexMatch = Regex.Match(trackingValue, pattern, RegexOptions.IgnoreCase);

                if (regexMatch.Success)
                {
                    string customerLevelFounded = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
                    return customerLevelFounded.Equals("4");
                }
            }

            return false;
        }
    }
}
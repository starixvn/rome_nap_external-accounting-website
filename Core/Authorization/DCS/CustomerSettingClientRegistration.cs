namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Fanex.CustomerSetting.Client40;

    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;
    using System.IO;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Fanex.Common;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return this.AllowRead();
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated();
        }

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated();
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

        private bool AllowUpdated()
        {
            bool isSuper = false;
            var trackingValue = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(trackingValue);
            if (!string.IsNullOrWhiteSpace(trackingValue))
            {
                var pattern = "(\"customerLevel\":{\"value\")([^\r\n][0-9]+)";
                Match regexMatch = Regex.Match(data["setting"], pattern, RegexOptions.IgnoreCase);

                if (regexMatch.Success)
                {
                    string customerLevelFounded = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
                    isSuper = customerLevelFounded.Equals("4"); 
                }
            }

            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            return isSuper ? false : _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.Sportsbook2Setting);
        }
    }
}
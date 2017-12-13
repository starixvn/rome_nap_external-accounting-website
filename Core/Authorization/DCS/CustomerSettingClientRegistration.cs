namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Utilities;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        private readonly ExternalAccountAuthorization authorization = new ExternalAccountAuthorization();

        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return this.AllowRead(permissionContext) && authorization.IsBelong();
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated(permissionContext) && authorization.IsBelong();
        }

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowUpdated(permissionContext) && authorization.IsBelong();
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
            settingContext.System =
                HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower().Contains("customersetting/betsetting") ||
                HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower().Contains("sssss=v") ? (byte)5 : settingContext.System;

            return settingContext;
        }

        private static bool AllowRead(PermissionContext permissionContext)
        {
            var authenticationService = new ExternalAccountAuthenticateAttribute();
            var productName = GetProductName(permissionContext);

            return string.IsNullOrWhiteSpace(productName)
                   || authenticationService.AllowReadByAuthenticationParam(productName);
        }

        private static bool AllowUpdated(PermissionContext permissionContext)
        {
            var isSuper = CheckIsSuperLevel(permissionContext);
            var authenticationService = new ExternalAccountAuthenticateAttribute();
            var productName = GetProductName(permissionContext);

            return (!isSuper || permissionContext.System != 4)
                   && (string.IsNullOrWhiteSpace(productName)
                       || authenticationService.AllowUpdateByAuthenticationParam(productName));
        }

        private static bool CheckIsSuperLevel(PermissionContext permissionContext)
        {
            var trackingValue = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("setting")
                .AttemptedValue;
            if (!string.IsNullOrWhiteSpace(trackingValue))
            {
                var pattern = "(\"customerLevel\":{\"value\")([^\r\n][0-9]+)";
                Match regexMatch = Regex.Match(trackingValue, pattern, RegexOptions.IgnoreCase);

                if (regexMatch.Success)
                {
                    var customerLevelFounded = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+",
                        string.Empty, RegexOptions.Compiled);
                    return customerLevelFounded.Equals("4");
                }
            }

            return false;
        }

        private static string GetProductName(PermissionContext permissionContext)
        {
            var queryString = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("productId");
            var val = queryString?.AttemptedValue;
            if (string.IsNullOrWhiteSpace(val))
            {
                var setting = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("setting");
                val = setting != null ? setting.AttemptedValue : string.Empty;
                const string pattern = "(\"productId\":)([^\r\n][0-9]*)";

                var regexMatch = Regex.Match(val, pattern, RegexOptions.IgnoreCase);

                if (regexMatch.Success)
                {
                    val = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+", string.Empty,
                        RegexOptions.Compiled);
                }
            }

            var productId = Utils.ToInt32(val);

            return GetProductFunction(productId, permissionContext.System);
        }

        private string GetProductFunction(int productId, byte systemId = 4)
        {
            return systemId == 4 ? Enum.GetName(typeof(ProductSettingDcsFunction), productId) : Enum.GetName(typeof(ProductBettingLimitDcsFunction), productId);
        }
    }
}
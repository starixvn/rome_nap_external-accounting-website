namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Context;
    using CustomerSetting.Client40;
    using CustomerSetting.Client40.Models;
    using Shared;
    using Utilities;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        private readonly AuthorizationService authorization = new AuthorizationService();


        public override bool VerifyQueryPermission(PermissionContext permissionContext) =>
            AllowRead(permissionContext) && authorization.IsBelong();

        public override bool VerifyUpdatePermission(PermissionContext permissionContext) =>
            AllowUpdated(permissionContext) && authorization.IsBelong();

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext) =>
            AllowUpdated(permissionContext) && authorization.IsBelong();

        public override SettingContext FetchSettingContext()
        {
            var currentSession = new CurrentSession();
            var settingContext = new SettingContext
            {
                ActorId = currentSession.UserId,
                ActorName = currentSession.UserName,
                ActorLevel = Role.Group,
                SubAccountId = 0,
                Language = CultureInfo.CurrentCulture.Name,
                Domain = HttpContext.Current.Request.Url.Host,
                IP = CurrentContext.Instance.Ip4Address
            };

            if (HttpContext.Current.Request.UrlReferrer == null)
            {
                return settingContext;
            }

            var pathAndQuery =
                HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower(CultureInfo.InvariantCulture);

            settingContext.System =
                HttpContext.Current.Request.UrlReferrer != null
                && (pathAndQuery.Contains("customersetting/betsetting")
                    || pathAndQuery.Contains("sssss=v"))
                    ? (byte) 5
                    : settingContext.System;

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

            if (string.IsNullOrWhiteSpace(trackingValue))
            {
                return false;
            }

            const string pattern = "(\"customerLevel\":{\"value\")([^\r\n][0-9]+)";
            var regexMatch = Regex.Match(trackingValue, pattern, RegexOptions.IgnoreCase);

            if (!regexMatch.Success)
            {
                return false;
            }

            var customerLevelFounded = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+",
                string.Empty, RegexOptions.Compiled);

            return customerLevelFounded.Equals(Role.Super.ToString());
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

        private static string GetProductFunction(int productId, byte systemId = 4) => systemId == 4
            ? Enum.GetName(typeof(ProductSettingDcsFunction), productId)
            : Enum.GetName(typeof(ProductBettingLimitDcsFunction), productId);
    }
}
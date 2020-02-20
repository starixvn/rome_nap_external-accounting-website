namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using Context;
    using CustomerSetting.Client40;
    using CustomerSetting.Client40.Models;
    using Shared;
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Utilities;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        private const byte SystemCode_ExternalNapBetSetting = 5;
        private const byte SystemCode_ExternalNapSpecialSite = 8;
        private const byte SystemCode_ExternalNap = 4;

        public override bool VerifyQueryPermission(PermissionContext permissionContext) =>
            AllowRead(permissionContext);

        public override bool VerifyUpdatePermission(PermissionContext permissionContext) =>
            AllowUpdated(permissionContext);

        public override bool VerifyMultipleUpdatePermission(PermissionContext permissionContext) =>
            AllowUpdated(permissionContext);

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

            settingContext.System = GetSystemCode(currentSession);

            return settingContext;
        }

        private byte GetSystemCode(CurrentSession currentSession)
        {
            if (IsBetSettingFunction())
            {
                return SystemCode_ExternalNapBetSetting;
            }

            if (currentSession.IsBelongToSpecialGroups)
            {
                return SystemCode_ExternalNapSpecialSite;
            }

            return SystemCode_ExternalNap;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private bool IsBetSettingFunction()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            var pathAndQuery =
               HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower(CultureInfo.InvariantCulture);
            return HttpContext.Current.Request.UrlReferrer != null
                && (pathAndQuery.Contains("customersetting/betsetting")
                    || pathAndQuery.Contains("sssss=v"));
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

            return (!isSuper || permissionContext.System != SystemCode_ExternalNap)
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
#pragma warning disable S109 // Magic numbers should not be used
                    val = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+", string.Empty,
#pragma warning restore S109 // Magic numbers should not be used
                        RegexOptions.Compiled);
                }
            }

            var productId = Utils.ToInt32(val);

            return GetProductFunction(productId, permissionContext.System);
        }

        private static string GetProductFunction(int productId, byte systemId = SystemCode_ExternalNap) => systemId == SystemCode_ExternalNap
            ? Enum.GetName(typeof(ProductSettingDcsFunction), productId)
            : Enum.GetName(typeof(ProductBettingLimitDcsFunction), productId);
    }
}
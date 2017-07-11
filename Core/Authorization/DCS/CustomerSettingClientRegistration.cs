namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Utilities;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return this.AllowRead(permissionContext);
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
            settingContext.System = 
                HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower().Contains("customersetting/betsetting") ||
                HttpContext.Current.Request.UrlReferrer.PathAndQuery.ToLower().Contains("sssss=v") ? (byte)5 : settingContext.System;

            return settingContext;
        }

        private bool AllowRead(PermissionContext permissionContext)
        {
            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            var productName = GetProductName(permissionContext);

            return string.IsNullOrWhiteSpace(productName) ? true : _authenticationService.AllowReadByAuthenticationParam(productName);
        }

        private bool AllowUpdated(PermissionContext permissionContext)
        {
            bool isSuper = this.CheckIsSuperLevel(permissionContext);
            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            var productName = GetProductName(permissionContext);

            return isSuper && permissionContext.System == 4 ? false : string.IsNullOrWhiteSpace(productName) ? true : 
                _authenticationService.AllowUpdateByAuthenticationParam(productName);
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

        private string GetProductName(PermissionContext permissionContext)
        {
            var queryString = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("productId");
            var val = queryString != null ? queryString.AttemptedValue : null;
            if (string.IsNullOrWhiteSpace(val))
            {
                var setting = permissionContext.AuthorizationContext.Controller.ValueProvider.GetValue("setting");
                val = setting != null ? setting.AttemptedValue : string.Empty;
                var pattern = "(\"productId\":)([^\r\n][0-9]+)";
                
                Match regexMatch = Regex.Match(val, pattern, RegexOptions.IgnoreCase);
            
                if (regexMatch.Success)
                {
                    val = Regex.Replace(regexMatch.Groups[2].ToString(), @"[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);                    
                }
            }
                        
            var productId = Utils.ToInt32(val);
            
            return GetProductFunction(productId, permissionContext.System);
        }

        private string GetProductFunction(int productId, byte systemId = 4)
        {
            switch (productId)
            {
                case 5:
                    return systemId == 4 ? eFunctions.VirtualSports : eFunctions.VirtualSportsBetting;
                case 16:
                    return systemId == 4 ? eFunctions.Sportsbook2Setting : eFunctions.Sportsbook2Betting;
                case 19:
                    return systemId == 4 ? eFunctions.CockfightingSetting : eFunctions.CockfightingBetting;
                case 20:
                    return systemId == 4 ? eFunctions.CricketSetting : eFunctions.CricketBetting;
                case 21:
                    return systemId == 4 ? eFunctions.AllbetSetting : eFunctions.AllbetBetting;
                case 22:
                    return systemId == 4 ? eFunctions.VoidbridgeSetting : eFunctions.VoidbridgeBetting;
                case 23:
                    return systemId == 4 ? eFunctions.NG2Setting : eFunctions.NG2Betting;
                case 24:
                    return eFunctions.FishingWorldSetting;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
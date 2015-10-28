namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System.Globalization;
    using System.Web;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        public override bool VerifyQueryPermission(PermissionContext permissionContext)
        {
            return this.AllowRead();
        }

        public override bool VerifyUpdatePermission(PermissionContext permissionContext)
        {
            return this.AllowRead();
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
            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.Sportsbook2Setting);
        }
    }
}
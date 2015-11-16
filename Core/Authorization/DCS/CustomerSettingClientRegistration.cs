namespace Fanex.ExternalAccount.Core.Authorization.DCS
{
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Fanex.CustomerSetting.Client40;
    using Fanex.CustomerSetting.Client40.Models;
    using Fanex.ExternalAccount.Core.Context;

    public class CustomerSettingClientRegistration : CustomerSettingRegistration
    {
        private bool isSuper = false;

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
            var queries = HttpContext.Current.Request.RawUrl.Split('&');
            if (queries.Length.Equals(1))
            {
                this.isSuper = HttpContext.Current.Request.Form["customerLevel"] != null ? HttpContext.Current.Request.Form["customerLevel"].Equals(4) : false;
            }
            else
            {
                this.isSuper = queries.Where(q => q.Contains("customerLevel")).First() != null ? queries.Where(q => q.Contains("customerLevel")).First().Split('=').ElementAt(1).Equals("4") : false;
            }

            ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
            return this.isSuper ? false : _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.Sportsbook2Setting);
        }
    }
}
namespace Fanex.ExternalAccount.UI.Services
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.LiveCasino.CoreSetting.Models;
    using Fanex.LiveCasino.CoreSetting.Services;

    /// <summary>
    /// Live casino authentication.
    /// </summary>
    public class LiveCasinoAuthorizationService : ILiveCasinoAuthorizationService
    {
        private ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, SettingContext settingContext)
        {
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.LiveCasino);
        }

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, SettingContext settingContext)
        {
            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.LiveCasino);
        }
    }
}
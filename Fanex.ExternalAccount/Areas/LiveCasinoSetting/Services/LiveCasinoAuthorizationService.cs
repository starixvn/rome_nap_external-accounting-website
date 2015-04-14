namespace Fanex.ExternalAccount.UI.Services
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.LiveCasino.CoreSetting.Enumerations;
    using Fanex.LiveCasino.CoreSetting.Models;
    using Fanex.LiveCasino.CoreSetting.Services;

    /// <summary>
    /// Live casino authentication.
    /// </summary>
    public class LiveCasinoAuthorizationService : ILiveCasinoAuthorizationService
    {
        private ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, LiveCasinoSettingContext settingContext)
        {
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.LiveCasino);
        }

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, LiveCasinoSettingContext settingContext)
        {
            if (settingContext.Customer.Level.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.LiveCasino);
        }

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, LiveCasinoSettingContextMultiple settingContextMultiple)
        {
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.LiveCasino);
        }

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, LiveCasinoSettingContextMultiple settingContextMultiple)
        {
            if (settingContextMultiple.Customer.Level.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.LiveCasino);
        }
    }
}
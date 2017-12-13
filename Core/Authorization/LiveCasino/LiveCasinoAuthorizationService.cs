namespace Fanex.ExternalAccount.Core.Authorization.LiveCasino
{
    using System.Web.Mvc;
    using Context;
    using Fanex.LiveCasino.CoreSetting.Enumerations;
    using Fanex.LiveCasino.CoreSetting.Models;
    using Fanex.LiveCasino.CoreSetting.Services;

    public class LiveCasinoAuthorizationService : ILiveCasinoAuthorizationService
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService =
            new ExternalAccountAuthenticateAttribute();

        private readonly ExternalAccountAuthorization authorization = new ExternalAccountAuthorization();

        public bool
            IsAuthorizedRead(AuthorizationContext authorizationContext, LiveCasinoSettingContext settingContext) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.LiveCasino)
            && authorization.IsBelong();

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, LiveCasinoSettingContext settingContext)
        {
            if (settingContext.Customer.Level.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.LiveCasino)
                   && authorization.IsBelong();
        }

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext,
            LiveCasinoSettingContextMultiple settingContextMultiple) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.LiveCasino)
            && authorization.IsBelong();

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext,
            LiveCasinoSettingContextMultiple settingContextMultiple)
        {
            if (settingContextMultiple.Customer.Level.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.LiveCasino)
                   && authorization.IsBelong();
        }
    }
}
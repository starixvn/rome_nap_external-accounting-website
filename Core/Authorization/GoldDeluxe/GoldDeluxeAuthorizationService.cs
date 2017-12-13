namespace Fanex.ExternalAccount.Core.Authorization.GoldDeluxe
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Context;
    using Fanex.GoldDeluxe.CoreSetting.Enumerations;
    using Fanex.GoldDeluxe.CoreSetting.Services;

    public class GoldDeluxeAuthorizationService : IGoldDeluxeAuthorizationService
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService =
            new ExternalAccountAuthenticateAttribute();

        private readonly ExternalAccountAuthorization _authorization = new ExternalAccountAuthorization();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, int custId) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe)
            && _authorization.IsBelong();

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, int custId)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe)
                   && _authorization.IsBelong();
        }

        public bool
            IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe)
            && _authorization.IsBelong();

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe)
                   && _authorization.IsBelong();
        }

        private string GetActionParameterValue(IValueProvider valueProvider, string parameterName) =>
            valueProvider.GetValue(parameterName) != null
                ? valueProvider.GetValue(parameterName).AttemptedValue
                : string.Empty;
    }
}
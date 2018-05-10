namespace Fanex.ExternalAccount.Core.Authorization.GoldDeluxe
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Context;
    using Fanex.GoldDeluxe.CoreSetting.Enumerations;
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using Shared;

    public class GoldDeluxeAuthorizationService : IGoldDeluxeAuthorizationService
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService =
            new ExternalAccountAuthenticateAttribute();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, int custId) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe);

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, int custId)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe);
        }

        public bool
            IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames) =>
            _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe);

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe);
        }

        private static string GetActionParameterValue(IValueProvider valueProvider, string parameterName) =>
            valueProvider.GetValue(parameterName) != null
                ? valueProvider.GetValue(parameterName).AttemptedValue
                : string.Empty;
    }
}
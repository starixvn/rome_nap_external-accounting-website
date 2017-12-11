namespace Fanex.ExternalAccount.Core.Authorization.GoldDeluxe
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.GoldDeluxe.CoreSetting.Enumerations;
    using Fanex.GoldDeluxe.CoreSetting.Services;

    public class GoldDeluxeAuthorizationService : IGoldDeluxeAuthorizationService
    {
        private ExternalAccountAuthenticate _authenticationService = new ExternalAccountAuthenticate();
        private readonly ExternalAccountAuthorization authorization = new ExternalAccountAuthorization();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, int custId)
        {
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe) && authorization.IsBelong(); ;
        }

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, int custId)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe) && authorization.IsBelong(); 
        }

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            return _authenticationService.AllowReadByAuthenticationParam(eFunctions.UpdateGoldDeluxe) && authorization.IsBelong();
        }

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            var valueProvider = authorizationContext.Controller.ValueProvider;
            var customerLevel = GetActionParameterValue(valueProvider, "level");
            if (customerLevel.Equals(CustomerLevel.Super))
            {
                return false;
            }

            return _authenticationService.AllowUpdateByAuthenticationParam(eFunctions.UpdateGoldDeluxe) && authorization.IsBelong();
        }

        private string GetActionParameterValue(IValueProvider valueProvider, string parameterName)
        {
            return valueProvider.GetValue(parameterName) != null
                                ? valueProvider.GetValue(parameterName).AttemptedValue
                                : string.Empty;
        }
    }
}

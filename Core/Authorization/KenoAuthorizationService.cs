namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using CustomerSetting.Module.Keno.KenoSetting.Services;
    using Fanex.ExternalAccount.Core.Context;

    public class KenoAuthorizationService : IKenoAuthorizationService
    {
        private readonly ExternalAccountAuthorization authorization = new ExternalAccountAuthorization();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext)
        {
            return authorization.IsBelong();
        }

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext)
        {
            return authorization.IsBelong();
        }

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            return authorization.IsBelong();
        }

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            return authorization.IsBelong();
        }
    }
}

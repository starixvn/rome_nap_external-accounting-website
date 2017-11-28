namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Module.ColossusBetSetting.Services;
    using Fanex.ExternalAccount.Core.Context;

    public class ColossusBetAuthorizationService : IColossusBetAuthorizationService
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

namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Context;
    using CustomerSetting.Module.ColossusBetSetting.Services;

    public class ColossusBetAuthorizationService : IColossusBetAuthorizationService
    {
        private readonly AuthorizationHandler authorization = new AuthorizationHandler();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext) => authorization.IsBelong();

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext) => authorization.IsBelong();

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext,
            IEnumerable<string> usernames) => authorization.IsBelong();

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext,
            IEnumerable<string> usernames) => authorization.IsBelong();
    }
}
namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Context;
    using CustomerSetting.Module.Keno.KenoSetting.Services;

    public class KenoAuthorizationService : IKenoAuthorizationService
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        public bool IsAuthorizedRead(AuthorizationContext authorizationContext) => authorization.IsBelong();

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext) => authorization.IsBelong();

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext,
            IEnumerable<string> usernames) => authorization.IsBelong();

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext,
            IEnumerable<string> usernames) => authorization.IsBelong();
    }
}
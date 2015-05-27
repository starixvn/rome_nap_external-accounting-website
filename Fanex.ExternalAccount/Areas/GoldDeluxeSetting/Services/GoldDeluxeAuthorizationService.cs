namespace Fanex.ExternalAccount.UI.Services
{
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class GoldDeluxeAuthorizationService : IGoldDeluxeAuthorizationService
    {
        public bool IsAuthorizedRead(AuthorizationContext authorizationContext, int custId)
        {
            return true;
        }

        public bool IsAuthorizedHost(AuthorizationContext authorizationContext, int custId)
        {
            return true;
        }

        public bool IsAuthorizedReadMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            return true;
        }

        public bool IsAuthorizedHostMultiple(AuthorizationContext authorizationContext, IEnumerable<string> usernames)
        {
            return true;
        }
    }
}

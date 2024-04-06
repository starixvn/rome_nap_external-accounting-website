using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Fanex.Customer.Client.NETFramework.Filters;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ExternalCustomerAuthFilterAttribute : CustomerAuthFilterAttribute, IAuthenticate
    {
        private readonly AuthorizationService authorization;

        private readonly IDictionary<int, string> ProductGroupPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.Sportsbook},
            { ProductGroupConstants.LiveAndRNGCasino, eFunctions.LiveAndRNGCasino}
        };

        private readonly IDictionary<int, string> ProductGroupBettingPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.SportsbookBetting},
            { ProductGroupConstants.LiveAndRNGCasino, eFunctions.LiveAndRNGCasinoBetting}
        };

        public ExternalCustomerAuthFilterAttribute()
            : base(null)
        {
            authorization = new AuthorizationServiceImpl();
        }

        public string Function { get; set; } = string.Empty;

        public int ProductGroupId { get; set; }

        public bool IsBettingLimit { get; set; }

        public AuthenticateAction.AuthenticationType Policy { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(Function))
            {
                var productGroupIdParam = IsHost
                ? filterContext.HttpContext.Request["productGroupId"]
                : GetActionParameterValue(filterContext.Controller.ValueProvider, "productGroupId");

                if (int.TryParse(productGroupIdParam, out int productGroupId))
                {
                    var permissions = IsBettingLimit ? ProductGroupBettingPermissions : ProductGroupPermissions;
                    Function = permissions.ContainsKey(productGroupId) ? permissions[productGroupId] : string.Empty;
                }
            }

            base.OnAuthorization(filterContext);

            Function = string.Empty;
        }

        public override bool AllowRead() => FunctionAuthorization(CurrentSession.Instance.UserId, Function, AuthenticateAction.AuthenticationType.Read);

        public override bool AllowHost() => FunctionAuthorization(CurrentSession.Instance.UserId, Function, AuthenticateAction.AuthenticationType.Host);

        public bool FunctionAuthorization(int userId, string functionKey, AuthenticateAction.AuthenticationType authenticateActionType) =>
            authorization.IsAuthorized(userId, functionKey, authenticateActionType, true);

        public bool AllowRead(string action)
            => AllowRead();

        public bool AllowUpdate(string action)
            => AllowHost();

        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true)
            => authorization.IsBelong(ExtRequest.Instance.GetGroupName(groupName), custId, username);

        private static string GetActionParameterValue(IValueProvider valueProvider, string parameterName)
        {
            var valueProviderResult = valueProvider.GetValue(parameterName);

            return valueProviderResult != null
                ? valueProviderResult.AttemptedValue
                : string.Empty;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Fanex.Customer.Client;
using Fanex.Customer.Client.Configurations;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ExternalCustomerAuthFilterAttribute : AuthorizeAttribute, IAuthenticate
    {
        private readonly AuthorizationService authorization;
        private readonly CustomerClientConfiguration clientConfig;

        public bool IsHost { get; set; }

        private readonly IDictionary<int, string> ProductGroupPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.Sportsbook},
            { ProductGroupConstants.RNGGames, eFunctions.RNGGames},
            { ProductGroupConstants.LiveAndRNGCasino, eFunctions.LiveAndRNGCasino}
        };

        private readonly IDictionary<int, string> ProductGroupBettingPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.SportsbookBetting},
            { ProductGroupConstants.RNGGames, eFunctions.RNGGamesBetting},
            { ProductGroupConstants.LiveAndRNGCasino, eFunctions.LiveAndRNGCasinoBetting}
        };

        public ExternalCustomerAuthFilterAttribute() : this(null)
        {
        }

        internal ExternalCustomerAuthFilterAttribute(CustomerClientConfiguration clientConfig)
        {
            this.clientConfig = clientConfig ?? CustomerClientManager.ClientConfigs;
            authorization = new AuthorizationServiceImpl();
        }

        public string Function { get; set; } = string.Empty;

        public int ProductGroupId { get; set; }

        public bool IsBettingLimit { get; set; }

        public AuthenticateAction.AuthenticationType Policy { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var isAuthorized = HasPermission(filterContext);

            if (!isAuthorized)
            {
                UnAuthorizedAction(filterContext);
            }
        }

        private bool HasPermission(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(Function))
            {
                return FunctionAuthorization(
                    CurrentSession.Instance.UserId,
                    Function,
                    IsHost ? AuthenticateAction.AuthenticationType.Host : AuthenticateAction.AuthenticationType.Read);
            }

            var productGroupIdParam = IsHost
                ? GetActionParameterValue(filterContext.Controller.ValueProvider, "productGroupId")
                : filterContext.HttpContext.Request["productGroupId"];

            if (int.TryParse(productGroupIdParam, out int productGroupId))
            {
                var permissions = IsBettingLimit ? ProductGroupBettingPermissions : ProductGroupPermissions;
                var function = permissions.ContainsKey(productGroupId) ? permissions[productGroupId] : string.Empty;
                return FunctionAuthorization(
                    CurrentSession.Instance.UserId,
                    function,
                    IsHost ? AuthenticateAction.AuthenticationType.Host : AuthenticateAction.AuthenticationType.Read);
            }

            return false;
        }

        public bool FunctionAuthorization(int userId, string functionKey, AuthenticateAction.AuthenticationType authenticateActionType) =>
            authorization.IsAuthorized(userId, functionKey, authenticateActionType, true);

        public bool AllowRead(string action)
             => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool AllowUpdate(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true)
            => authorization.IsBelong(ExtRequest.Instance.GetGroupName(groupName), custId, username);

        private static string GetActionParameterValue(IValueProvider valueProvider, string parameterName)
        {
            var valueProviderResult = valueProvider.GetValue(parameterName);

            return valueProviderResult != null
                ? valueProviderResult.AttemptedValue
                : string.Empty;
        }

        private void UnAuthorizedAction(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest()
                || filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                response.Clear();
                response.Write("{ \"message\": \"You are not authorized on this function!\" }");
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(clientConfig.AccessDeniedUrl);
            }
        }
    }
}
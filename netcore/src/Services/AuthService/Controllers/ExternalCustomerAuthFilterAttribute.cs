using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Customer.Client;
using Fanex.Customer.Client.Configurations;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ExternalCustomerAuthFilterAttribute : AuthorizeAttribute, IAuthorizationFilter, IAuthenticate
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

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthorized = HasPermission(context);

            if (!isAuthorized)
            {
                UnAuthorizedAction(context);
            }
        }

        private bool HasPermission(AuthorizationFilterContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(Function))
            {
                return FunctionAuthorization(
                    CurrentSession.Instance.UserId,
                    Function,
                    IsHost ? AuthenticateAction.AuthenticationType.Host : AuthenticateAction.AuthenticationType.Read);
            }

            var productGroupIdParam = IsHost
                ? GetActionParameterValue(filterContext, "productGroupId")
                : filterContext.HttpContext.Request.Query["productGroupId"].ToString();

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

        private static string GetActionParameterValue(AuthorizationFilterContext filterContext, string parameterName)
        {
            // Try to get from route values
            if (filterContext.RouteData.Values.TryGetValue(parameterName, out var routeValue))
            {
                return routeValue?.ToString() ?? string.Empty;
            }

            // Try to get from query string
            if (filterContext.HttpContext.Request.Query.TryGetValue(parameterName, out var queryValue))
            {
                return queryValue.ToString();
            }

            return string.Empty;
        }

        private void UnAuthorizedAction(AuthorizationFilterContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest"
                || filterContext.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                response.Clear();
                response.ContentType = "application/json";
                response.StatusCode = 401;
                filterContext.Result = new JsonResult(new { message = "You are not authorized on this function!" });
            }
            else
            {
                filterContext.Result = new RedirectResult(clientConfig.AccessDeniedUrl);
            }
        }
    }
}

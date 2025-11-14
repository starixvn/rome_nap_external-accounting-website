using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fanex.Customer.Client;
using Fanex.Customer.Client.Configurations;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System.Linq;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    // Đã fix lại class ExternalCustomerAuthFilterAttribute cho .NET Core
    public class ExternalCustomerAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter, IAuthenticate
    {
        private readonly AuthorizationService authorization;
        private readonly CustomerClientConfiguration clientConfig;

        public bool IsHost { get; set; }

        private static readonly IDictionary<int, string> ProductGroupPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.Sportsbook }
            // Chú ý: Đã loại bỏ các keys không còn tồn tại trong eFunctions (xem lint)
        };

        private static readonly IDictionary<int, string> ProductGroupBettingPermissions = new Dictionary<int, string>
        {
            { ProductGroupConstants.Sportsbook, eFunctions.SportsbookBetting }
            // Chú ý: Đã loại bỏ các keys không còn tồn tại trong eFunctions (xem lint)
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

        // .NET Core version of OnAuthorization
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!await HasPermissionAsync(context))
            {
                UnAuthorizedAction(context);
            }
        }

        private async Task<bool> HasPermissionAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            if (!string.IsNullOrWhiteSpace(Function))
            {
                return FunctionAuthorization(
                    CurrentSession.Instance.UserId,
                    Function,
                    IsHost ? AuthenticateAction.AuthenticationType.Host : AuthenticateAction.AuthenticationType.Read);
            }

            string productGroupIdParam = null;
            if (IsHost)
            {
                // Try get from ActionArguments if present (for POST/ModelBinding)
                if (context.ActionDescriptor.Parameters.Any(x => x.Name == "productGroupId"))
                {
                    if (context.HttpContext.Items.TryGetValue("productGroupId", out var argValue) && argValue != null)
                    {
                        productGroupIdParam = argValue.ToString();
                    }
                }
            }
            else
            {
                // Try from Query or Form
                if (httpContext.Request.Query.ContainsKey("productGroupId"))
                    productGroupIdParam = httpContext.Request.Query["productGroupId"];
                else if (httpContext.Request.HasFormContentType && httpContext.Request.Form.ContainsKey("productGroupId"))
                    productGroupIdParam = httpContext.Request.Form["productGroupId"];
            }

            if (int.TryParse(productGroupIdParam, out int productGroupId))
            {
                var permissions = IsBettingLimit ? ProductGroupBettingPermissions : ProductGroupPermissions;
                // Tìm function theo group id
                var function = permissions.TryGetValue(productGroupId, out var value) ? value : string.Empty;
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

        // Không còn dùng ValueProvider trong ASP.NET Core, thay bằng ActionArguments như bên trên

        private void UnAuthorizedAction(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            if (IsAjaxRequest(httpContext.Request) || httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new JsonResult(new
                {
                    message = "You are not authorized on this function!"
                })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                context.Result = new RedirectResult(clientConfig.AccessDeniedUrl);
            }
        }

        // Helper to check Ajax request in ASP.NET Core
        private static bool IsAjaxRequest(HttpRequest request)
        {
            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return false;
        }
    }
}
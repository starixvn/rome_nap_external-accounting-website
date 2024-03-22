using System;
using Fanex.Customer.Client.NETFramework.Filters;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ExternalCustomerAuthFilterAttribute : CustomerAuthFilterAttribute, IAuthenticate
    {
        private readonly AuthorizationService authorization;

        public ExternalCustomerAuthFilterAttribute()
            : base(null)
        {
            authorization = new AuthorizationServiceImpl();
        }

        public string Function { get; set; }

        public AuthenticateAction.AuthenticationType Policy { get; set; }

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
    }
}
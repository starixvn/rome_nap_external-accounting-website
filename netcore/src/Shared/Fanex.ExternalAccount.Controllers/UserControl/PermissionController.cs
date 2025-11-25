namespace Fanex.ExternalAccount.Controllers.UserControl
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.UserControl;
    using Newtonsoft.Json;
    using Service.UserControl;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class PermissionController : BaseController
    {
        private readonly IPolicyService policyService;

        public PermissionController(IPolicyService policyService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService)
         : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.policyService = policyService;
        }

        [ExternalAccountAuthenticate(
            Function = eFunctions.GrantPermission,
            Policy = AuthenticateAction.AuthenticationType.Read,
            IsAjaxRequest = false)]
        public IActionResult Index(int? funcId)
        {
            ViewBag.FuncId = funcId;

            return PartialViewBase(policyService.GetHierarchicalFunctions());
        }

        [ExternalAccountAuthenticate(
            Function = eFunctions.GrantPermission,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public IActionResult GetGroupPolicies(int funcId)
        {
            var groupPolicies = policyService.GetGroupPolicies(funcId);

            if (!CurrentSession.IsITAdmin)
            {
                groupPolicies = CurrentSession.IsInternalAdmin
                    ? groupPolicies.Where(o => !Group.IsITGroup(o.GroupId))
                    : groupPolicies.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return JsonBase(groupPolicies, GetJsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(
            Function = eFunctions.GrantPermission,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public IActionResult GetUserPolicies(int funcId)
        {
            var userPolicies = policyService.GetUserPolicies(funcId);

            if (!CurrentSession.IsITAdmin)
            {
                userPolicies = CurrentSession.IsInternalAdmin
                    ? userPolicies.Where(o => !Group.IsITGroup(o.GroupId))
                    : userPolicies.Where(o => o.GroupName == CurrentSession.CurrencyGroup);
            }

            return JsonBase(userPolicies, GetJsonSerializerSettings());
        }

        [ValidateAntiForgeryToken]
        [ExternalAccountAuthenticate(
            Function = eFunctions.GrantPermission,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public IActionResult UpdateGroupPolicy(int funcId, string policies)
            => JsonBase(
                policyService.UpdateMultipleGroupPolicies(funcId, policies, CurrentSession.UserId, CurrentContext.Ip4Address),
                GetJsonSerializerSettings());

        [ValidateAntiForgeryToken]
        [ExternalAccountAuthenticate(
            Function = eFunctions.GrantPermission,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public IActionResult UpdateUserPolicy(int funcId, string policies)
            => JsonBase(
                policyService.UpdateMultipleUserPolicies(funcId, policies, CurrentSession.UserId, CurrentContext.Ip4Address),
                GetJsonSerializerSettings());

        private static JsonSerializerSettings GetJsonSerializerSettings()
            => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
    }
}

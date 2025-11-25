namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Service.Impl.Customer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    [ExternalAccountAuthenticate(Function = eFunctions.AssociationAccountList, Policy = AuthenticationType.Read)]
    public class AssociationAccountController : BaseController
    {
        private readonly IAssociationAccountService associationService;

        public AssociationAccountController(
            IAssociationAccountService associationService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            IAuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.associationService = associationService;
        }

        public IActionResult Index()
            => PartialViewBase();


        public IActionResult Get(GetAssociationAccountCriteria criteria)
        {
            criteria.GroupName = ExtRequest.GetGroupName();
            criteria.UserId = CurrentSession.UserId;
            var accounts = associationService.Get(criteria);

            return Ok(accounts);
        }

    }
}
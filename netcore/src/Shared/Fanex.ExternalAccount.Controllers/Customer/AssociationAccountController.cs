namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer;
    using Microsoft.AspNetCore.Mvc;
    using Service.Impl.Customer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    [ExternalAccountAuthenticate(Function = eFunctions.AssociationAccountList, Policy = AuthenticationType.Read)]
    public class AssociationAccountController : BaseController
    {
        private readonly IAssociationAccountService associationService;

        public AssociationAccountController() : this(null)
        {

        }

        public AssociationAccountController(IAssociationAccountService associationService)
        {
            this.associationService = associationService ?? new AssociationAccountService();
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
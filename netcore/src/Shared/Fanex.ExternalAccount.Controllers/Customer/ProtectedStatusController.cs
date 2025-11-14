namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Authorization;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.ExternalAccount.Utilities;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class ProtectedStatusController : BaseController
    {
        private readonly ICustomerService customerService;

        public ProtectedStatusController() : this(new CustomerService())
        {
        }

        internal ProtectedStatusController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.ProtectCustomer, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult OpenMultiple([FromQuery] string groupName, [FromQuery] string custIds, [FromQuery] byte roleId)
            => PartialViewBase(customerService.GetMultipleProtectedStatus(groupName, custIds, roleId, CurrentSession.UserId));

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExternalAccountAuthenticate(Function = eFunctions.ProtectCustomer, Policy = AuthenticationType.Host, IsAjaxRequest = true)]
        public IActionResult EditMultiple([FromQuery] string groupName, [FromBody] CustomerProtectedStatus[] protectedStatus)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new System.ArgumentNullException(nameof(groupName));
            }

            var custIds = protectedStatus.Select(item => item.CustId);

            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return JsonBase2(new Result((int)HttpStatusCode.Forbidden, ViewLang.DontHavePermission));
            }

            var results = customerService.UpdateMultipleProtectedStatus(
                protectedStatus,
                CurrentSession.UserId,
                CurrentContext.Ip4Address);

            return JsonBase2(results);
        }
    }
}

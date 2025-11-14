namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Requests.Customer.OpenCustomer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    public partial class CustomerController
    {
        private const string singleOpenCustomerViewName = "SingleOpenCustomer";
        private const string multipleOpenCustomerViewName = "MultipleOpenCustomer";

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        public IActionResult SingleOpenCustomerView(GetSingleOpenCustomerInfoQuery query)
            => PartialViewBase(_customerService.GetSingleOpenCustomerInfo(query), singleOpenCustomerViewName);

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        public IActionResult MultipleOpenCustomerView(GetMultipleOpenCustomerInfoQuery query)
            => PartialViewBase(_customerService.GetMultipleOpenCustomerInfo(query), multipleOpenCustomerViewName);

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult GetSingleOpenDownlinesInfo([FromBody] GetSingleOpenDownlinesInfoQuery query)
        {
            var result = _customerService.GetSingleOpenDownlinesInfo(query);
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Host)]
        public IActionResult SingleOpenCustomer([FromBody] UpdateSingleClosedStatusQuery query)
        {
            query.UserId = Core.Context.CurrentSession.Instance.UserId;
            query.Language = Core.Context.CurrentSession.Instance.LangKey;
            query.Ip = Core.Context.CurrentContext.Instance.Ip4Address;

            var result = _customerService.UpdateSingleCustomerClosedStatus(query);
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Host)]
        public IActionResult MultipleOpenCustomer(
            [FromBody] IEnumerable<UpdateMultipleClosedStatusQuery> request,
            [FromQuery] string groupName,
            [FromQuery] int roleId)
        {
            var custIds = request.Select(item => item.CustId);
            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return Ok(new Result(1, "You are not authorized on this function!"));
            }

            foreach (var item in request)
            {
                item.UserId = Core.Context.CurrentSession.Instance.UserId;
                item.Language = Core.Context.CurrentSession.Instance.LangKey;
            }

            var result = _customerService.UpdateMultipleCustomerClosedStatus(request, roleId);
            return Ok(result);
        }
    }
}
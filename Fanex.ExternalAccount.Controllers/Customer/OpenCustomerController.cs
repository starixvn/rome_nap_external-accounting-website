namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Requests.Customer.OpenCustomer;
    using System.Web.Mvc;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    public partial class CustomerController
    {
        private const string singleOpenCustomerViewName = "SingleOpenCustomer";

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        public ActionResult SingleOpenCustomerView(GetSingleOpenCustomerInfoQuery query)
            => PartialViewBase(_customerService.GetSingleOpenCustomerInfo(query), singleOpenCustomerViewName);

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer,
            Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetSingleOpenDownlinesInfo(GetSingleOpenDownlinesInfoQuery query)
            => Json(_customerService.GetSingleOpenDownlinesInfo(query));

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer,
            Policy = AuthenticationType.Host)]
        public JsonResult SingleOpenCustomer(UpdateSingleClosedStatusQuery query)
            => Json(_customerService.UpdateSingleCustomerClosedStatus(query));
    }
}
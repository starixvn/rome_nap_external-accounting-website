namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
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
        public ActionResult SingleOpenCustomerView(GetSingleOpenCustomerInfoQuery query)
            => PartialViewBase(_customerService.GetSingleOpenCustomerInfo(query), singleOpenCustomerViewName);

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        public ActionResult MultipleOpenCustomerView(GetMultipleOpenCustomerInfoQuery query)
            => PartialViewBase(_customerService.GetMultipleOpenCustomerInfo(query), multipleOpenCustomerViewName);

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetSingleOpenDownlinesInfo(GetSingleOpenDownlinesInfoQuery query)
            => Json(_customerService.GetSingleOpenDownlinesInfo(query));

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Host)]
        public JsonResult SingleOpenCustomer(UpdateSingleClosedStatusQuery query)
        {
            query.UserId = Core.Context.CurrentSession.Instance.UserId;
            query.Language = Core.Context.CurrentSession.Instance.LangKey;
            query.Ip = Core.Context.CurrentContext.Instance.Ip4Address;

            return Json(_customerService.UpdateSingleCustomerClosedStatus(query));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticationType.Host)]
        public JsonResult MultipleOpenCustomer(
            IEnumerable<UpdateMultipleClosedStatusQuery> request,
            string groupName,
            int roleId)
        {
            var custIds = request.Select(item => item.CustId);
            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return Json(new Result(1, "You are not authorized on this function!"));
            }

            foreach (var item in request)
            {
                item.UserId = Core.Context.CurrentSession.Instance.UserId;
                item.Language = Core.Context.CurrentSession.Instance.LangKey;
            }

            return Json(_customerService.UpdateMultipleCustomerClosedStatus(request, roleId));
        }
    }
}
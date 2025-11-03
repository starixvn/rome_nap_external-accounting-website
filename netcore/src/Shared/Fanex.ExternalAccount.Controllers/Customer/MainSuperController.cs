namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using Service.Impl.Customer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    public class MainSuperController : BaseController
    {        
        private readonly SuperService _superService = new SuperService();

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]        
        public ActionResult EditInfo(int custId)
        {
            var model = _superService.GetSuperInfo(custId);

            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.Phone = HttpUtility.HtmlDecode(model.Phone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);
            model.Remark = HttpUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public ActionResult GetInfo(int superId)
        {
            var model = _superService.GetSuperInfo(superId);

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]        
        public ActionResult EditInfoPost(CustomerInfo customer)
        {
            var criteria = new UpdateSuperInfoCommand(
                customer.CustId,
                customer.FirstName,
                customer.LastName,
                customer.Phone,
                customer.MobilePhone,
                customer.Fax,
                customer.Remark,
                customer.Email,
                CurrentContext.Ip4Address,
                CurrentSession.CustId);
            var result = _superService.UpdateSuperInfo(criteria);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

    }
}
namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class MainSuperController : BaseController
    {
        private readonly ISuperService superService;

        public MainSuperController(ISuperService superService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.superService = superService;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]        
        public IActionResult EditInfo(int custId)
        {
            var model = superService.GetSuperInfo(custId);

            model.FirstName = WebUtility.HtmlDecode(model.FirstName);
            model.LastName = WebUtility.HtmlDecode(model.LastName);
            model.Phone = WebUtility.HtmlDecode(model.Phone);
            model.MobilePhone = WebUtility.HtmlDecode(model.MobilePhone);
            model.Email = WebUtility.HtmlDecode(model.Email);
            model.Fax = WebUtility.HtmlDecode(model.Fax);
            model.Remark = WebUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public IActionResult GetInfo(int superId)
        {
            var model = superService.GetSuperInfo(superId);

            return JsonBase(model);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]        
        public IActionResult EditInfoPost([FromBody] CustomerInfo customer)
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
            var result = superService.UpdateSuperInfo(criteria);

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

    }
}
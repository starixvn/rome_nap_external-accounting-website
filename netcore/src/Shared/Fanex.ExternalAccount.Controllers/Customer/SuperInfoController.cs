namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Requests.Customer.SuperList;
    using Fanex.ExternalAccount.Service.Customer;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class SuperInfoController : BaseController
    {
        private readonly IEditSuperListService _superListSevice;

        public SuperInfoController(IEditSuperListService superListSevice, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService)
         : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _superListSevice = superListSevice;
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public IActionResult Index()
        {

            ViewBag.SCode = Encryptor.RandomCaptcha;

            return PartialViewBase();
        }
        
        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public IActionResult CheckMultipleSupers([FromBody] string destUsers, [FromBody] string groupName)
        {
            var result = _superListSevice.CheckMultipleSupers(destUsers, groupName, CurrentSession.UserId);
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, SuperIds = result.ResultData },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public IActionResult UpdateMultipleSupers([FromBody] UpdateMultipleSuperRequest request)
        {
            request.Password = Encryptor.Decrypt(request.Password, Encryptor.RandomCaptcha);
            var result = _superListSevice.UpdateMultipleSuperRequest(request, CurrentSession.UserId, CurrentContext.Ip4Address);
            return JsonBase(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}

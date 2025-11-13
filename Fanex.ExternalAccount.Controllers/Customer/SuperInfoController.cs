namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Requests.Customer.SuperList;
    using Fanex.ExternalAccount.Service.Customer;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Web.Mvc;

    public class SuperInfoController : BaseController
    {
        private readonly IEditSuperListService superListSevice;

        public SuperInfoController(): this(null)
        {

        }

        public SuperInfoController(IEditSuperListService superListSevice)
        {
            this.superListSevice = superListSevice ?? new EditSuperListService();
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public ActionResult Index()
        {

            ViewBag.SCode = Encryptor.RandomCaptcha;

            return PartialViewBase();
        }
        
        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public ActionResult CheckMultipleSupers(string destUsers, string groupName)
        {
            var result = superListSevice.CheckMultipleSupers(destUsers, groupName, CurrentSession.UserId);
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, SuperIds = result.ResultData },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public ActionResult UpdateMultipleSupers(UpdateMultipleSuperRequest request)
        {
            request.Password = Encryptor.Decrypt(request.Password, Encryptor.RandomCaptcha);
            var result = superListSevice.UpdateMultipleSuperRequest(request, CurrentSession.UserId, CurrentContext.Ip4Address);
            return JsonBase(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "EditSuperInfo")]
        public ActionResult UpdateHideBetIP(int superCustId, bool isHideBetIP)
        {
            var result = superListSevice.UpdateHideBetIP(superCustId, isHideBetIP, CurrentSession.UserId, CurrentContext.Ip4Address);
            return JsonBase(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}

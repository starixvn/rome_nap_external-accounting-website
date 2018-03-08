using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Authorization.ThirdPartyStatus;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Service.Impl.Customer;
using Fanex.ExternalAccount.Service.Interface.Customer;
using Fanex.ExternalAccount.Utilities;
using ThirdPartyProducts.StatusSetting.Model;

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    public class ThirdPartyProductStatusController : BaseController
    {
        private const string AccessDeniedView = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml";
        private const string EnableThirdPartyProductStatusView = "~/Areas/CustomerSetting/Views/EnableThirdPartyProductStatus/Index.cshtml";
        private readonly IThirdPartyProductStatusService thirdPartyProductStatusService;
        private readonly ProductStatusAuthorization authenticationService;

        public ThirdPartyProductStatusController()
        {
            thirdPartyProductStatusService = new ThirdPartyProductStatusService();
            authenticationService = new ProductStatusAuthorization();
        }

        public ActionResult Index(int custId, string custName, int roleId, int productId)
        {
            var isAllowed = authenticationService.HasPermission(AuthenticateAction.AuthenticationType.Read, productId);
            if (!isAllowed)
            {
                return View(AccessDeniedView);
            }

            return View(
                EnableThirdPartyProductStatusView,
                thirdPartyProductStatusService.Select(
                    custId, 
                    custName,
                    roleId,
                    productId
                    ));
        }

        public JsonResult Update(
            int custId,
            int productId,
            string productStatus)
        {
            var isAllowed = authenticationService.HasPermission(AuthenticateAction.AuthenticationType.Host, productId);
            if (!isAllowed)
            {
                var deniedResult = new List<ProductStatusResult>
                {
                    new ProductStatusResult
                    {
                        ErrorCode = "999",
                        ErrorMessage = ViewLang.DontHavePermission
                    }
                };

                return Json(deniedResult);
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            var productStatusModel = js.Deserialize<List<ThirdPartyProducts.StatusSetting.Model.ProductStatus>>(productStatus);
            var result = thirdPartyProductStatusService.Update(
                custId,
                0,
                productId,
                CurrentSession.UserId,
                Config.SystemId,
                CurrentContext.Ip4Address,
                CurrentSession.UserName,
                productStatusModel);            

            return Json(result);
        }
    }
}

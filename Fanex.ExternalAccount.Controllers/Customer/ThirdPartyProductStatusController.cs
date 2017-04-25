using Fanex.Tarus.Business.Implement.Customer;
using System.Collections.Generic;
using System.Web.Mvc;
using ThirdPartyProducts.StatusSetting.Model;
using System.Web.Script.Serialization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Utilities;
using Fanex.ExternalAccount.Core.Base;

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    public class ThirdPartyProductStatusController : BaseController
    {
        private ThirdPartyProductStatusService thirdPartyProductStatusService;

        public ThirdPartyProductStatusController()
        {
            thirdPartyProductStatusService = new ThirdPartyProductStatusService();
        }

        public ActionResult Index(int custId, string custName, int roleId, int productId)
        {
            return View(
                "~/Areas/CustomerSetting/Views/EnableThirdPartyProductStatus/Index.cshtml", 
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
            JavaScriptSerializer js = new JavaScriptSerializer();
            
            return Json(thirdPartyProductStatusService.Update(
                custId,
                0,
                productId,
                CurrentSession.UserId,
                Config.SystemId,
                CurrentContext.Ip4Address,
                js.Deserialize<List<ProductStatus>>(productStatus)));
        }
    }
}

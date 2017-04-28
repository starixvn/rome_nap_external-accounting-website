using CasinoLibrary.Model.UserException;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Utilities;
using Fanex.Module.Logs;
using Fanex.Module.Logs.Services;
using Fanex.Tarus.Business.Implement.Customer;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThirdPartyProducts.StatusSetting.Model;

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
            var productStatusModel = js.Deserialize<List<ProductStatus>>(productStatus);
            var result = thirdPartyProductStatusService.Update(
                custId,
                0,
                productId,
                CurrentSession.UserId,
                Config.SystemId,
                CurrentContext.Ip4Address,
                productStatusModel);

            if (result[0].ErrorCode.Equals("0"))
            {
                var customer = new CasinoLibrary.SBAgent.Customer();
                try
                {
                    customer.UpdateCustEnableCasino(
                        custId,
                        !productStatusModel[0].IsDisabled,
                        !productStatusModel[0].IsDisabledDownline,
                        CurrentContext.Ip4Address,
                        CurrentSession.UserId,
                        CurrentSession.UserName,
                        3);
                }
                catch (ErrorException casinoEx)
                {
                    casinoEx.WriteCasinoException(Formater.LogType.Warning);
                }
                catch (Exception ex)
                {
                    ex.Write(Formater.LogType.Warning);
                }
            }

            return Json(result);
        }
    }
}

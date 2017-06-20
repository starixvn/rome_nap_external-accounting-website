using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CasinoLibrary.Model.UserException;
using Fanex.ExternalAccount.Core.Authorization.ThirdPartyStatus;
using Fanex.ExternalAccount.Core.Base;
using Fanex.ExternalAccount.Utilities;
using Fanex.Module.Logs;
using Fanex.Module.Logs.Services;
using Fanex.Tarus.Business.Implement.Customer;
using ThirdPartyProducts.StatusSetting.Enums;
using ThirdPartyProducts.StatusSetting.Model;

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    public class ThirdPartyProductStatusController : BaseController
    {
        private readonly ThirdPartyProductStatusService thirdPartyProductStatusService;
        private readonly ProductStatusAuthorization authenticationService;

        public ThirdPartyProductStatusController()
        {
            thirdPartyProductStatusService = new ThirdPartyProductStatusService();
            authenticationService = new ProductStatusAuthorization();
        }

        public ActionResult Index(int custId, string custName, int roleId, int productId)
        {
            var isAllowed = authenticationService.HasPermission(CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType.Read, productId);
            if (!isAllowed)
            {
                return View("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

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
            var isAllowed = authenticationService.HasPermission(CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType.Host, productId);
            if (!isAllowed)
            {
                var deniedResult = new List<ProductStatusResult>();
                deniedResult.Add(new ProductStatusResult
                {
                    ErrorCode = "999",
                    ErrorMessage = ViewLang.DontHavePermission
                });

                return Json(deniedResult);
            }

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

            if (productId == (int)Product.Casino && result[0].ErrorCode.Equals("0"))
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

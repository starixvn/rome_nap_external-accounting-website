namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Web.Mvc;
    using Base;
    using Casino.Entities;
    using Casino.Services.Impl;
    using Core.Web;
    using ExternalAccount.Core.Shared;
    using Fanex.CustomerSetting.Module.Core.Constants;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Security.Authenticate;
    using Security.Configurations;
    using ViewModels;

    public class CasinoBettingController : BettingControllerBase
    {
        public ActionResult Index(int custId, int uplineId, int userId, string groupName)
           => View("~/Areas/CustomerSetting/Views/BetSetting/Casino/Index.cshtml");

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "CasinoBetting")]
        public ActionResult Content(int custId, int uplineId, int userId, string groupName)
        {
            var customer = CustomerInfo(custId, userId);

            if (customer.RoleId.IsNotSuper() || !customer.SyncCasino)
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            CasinoModel model;

            try
            {
                var settingModel = superCasinoService.SuperCasinoModel(custId, uplineId);
                var customerModel = casinoServiceBaseImpl.GetCustInfoIBC(custId, uplineId);

                var customerUpline = new CasinoCustomer
                {
                    GeneralInfo = { IBCCustID = uplineId }
                };

                customerUpline = casinoServiceBaseImpl.GetCustSettingCS(customerUpline);

                customerModel.GeneralInfo.IBCCustID = customerModel.GeneralInfo.CasinoSync ? custId : uplineId;
                customerModel = casinoServiceBaseImpl.GetCustSettingCS(customerModel);

                customerModel.GeneralInfo.UplineRoleId = customerUpline.GeneralInfo.RoleId;

                model = new CasinoModel
                {
                    CustomerModel = customerModel,
                    SettingModel = settingModel
                };
            }
            catch
            {
                model = null;
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            return View("~/Areas/CustomerSetting/Views/BetSetting/Casino/Content.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "CasinoBetting")]
        public ActionResult EditBetSettingPost(int custId, int uplineId, int userId, string txtCustomerObject)
        {
            var checkUmResult = serviceBase.CheckUM(custId, NonDcsProduct.RNGCasino, AppSession.CurrentSession.LangKey);
            if (!checkUmResult.IsSuccess)
            {
                return UmJsonResult(checkUmResult);
            }

            var customer = CustomerInfo(custId, userId);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (customer.RoleId.IsNotSuper() || !customer.SyncCasino)
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }


            var model = JsonConvert.DeserializeObject<CasinoModel>(txtCustomerObject);
            var result = superCasinoService.UpdateSuperBettingLimit(
                custId,
                4,
                Request.UserHostAddress,
                userId,
                customer.Username,
                AppSession.CurrentSession.FromSystem,
                model.SettingModel);

            return JsonBase(result, settings);
        }
    }
}
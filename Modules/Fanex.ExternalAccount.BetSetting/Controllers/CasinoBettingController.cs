namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Fanex.CustomerSetting.Module.BetSetting.Base;
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;
    using Fanex.CustomerSetting.Module.Casino.Entities;
    using Fanex.CustomerSetting.Module.Casino.Services.Impl;
    using Fanex.CustomerSetting.Module.Casino.ViewModels;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.CustomerSetting.Security.Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class CasinoBettingController : BettingControllerBase
    {
        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return this.ActionDenied(this.Request);
            }

            return this.View("~/Areas/CustomerSetting/Views/BetSetting/Casino/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "CasinoBetting")]
        public ActionResult Content(int id, int uplineId, int userId, string siteName)
        {
            var customer = this.CustomerInfo(id, userId);

            if (customer.RoleId.IsNotSuper() ||  !customer.SyncCasino)
            {
                return this.PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            CasinoSettingModel settingModel = null;
            CasinoCustomer customerModel = null;
            CasinoModel model = null;

            try
            {
                settingModel = CasinoManager.Instance.SuperCasinoModel(id, uplineId);
                customerModel = CasinoManager.Instance.GetCustInfoIBC(id, uplineId);

                var customerUpline = new CasinoCustomer();

                customerUpline.GeneralInfo.IBCCustID = uplineId;
                CasinoManager.Instance.GetCustSettingCS(ref customerUpline);

                customerModel.GeneralInfo.IBCCustID = customerModel.GeneralInfo.CasinoSync ? id : uplineId;
                CasinoManager.Instance.GetCustSettingCS(ref customerModel);

                customerModel.GeneralInfo.UplineRoleId = customerUpline.GeneralInfo.RoleId;

                model = new CasinoModel
                {
                    CustomerModel = customerModel,
                    SettingModel = settingModel
                };
            }
            catch (Exception)
            {
                model = null;
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.SyncBingo = customer.SyncBingo;
            ViewBag.SyncLiveCasino = customer.SyncLiveCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            return this.View("~/Areas/CustomerSetting/Views/BetSetting/Casino/Content.cshtml", model);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "CasinoBetting")]
        public ActionResult EditBetSettingPost(int id, int uplineId, int userId, string txtCustomerObject, string siteName = "ACCUT")
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return this.ActionDenied(this.Request);
            }

            var customer = this.CustomerInfo(id, userId);

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if (customer.RoleId.IsNotSuper() ||  !customer.SyncCasino)
            {
                return this.JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            CasinoModel model = JsonConvert.DeserializeObject<CasinoModel>(txtCustomerObject);
            var  result = CasinoManager.Instance.UpdateSuperBettingLimit(
                id, 
                4, 
                Request.UserHostAddress, 
                userId, 
                customer.Username,
                AppSession.CurrentSession.FromSystem, 
                model.SettingModel, 
                model.CustomerModel);

            return this.JsonBase(result, settings);
        }
    }
}
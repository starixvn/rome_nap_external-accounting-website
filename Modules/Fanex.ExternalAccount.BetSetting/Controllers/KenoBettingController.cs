namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Base;
    using Keno.KenoSetting.Configurations;
    using Security.Authenticate;
    using Security.Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Fanex.CustomerSetting.Module.Core.Constants;
    using Fanex.CustomerSetting.Module.Core.Web;

    public class KenoBettingController : BettingControllerBase
    {
        public ActionResult Index(int custId, int uplineId, int userId, string groupName)
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/Keno/Index.cshtml");

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "KenoBetting")]
        public ActionResult Content(int custId, int uplineId, int userId, string groupName)
        {
            var customer = CustomerInfo(custId, userId);

            if (customer.RoleId.IsNotSuper())
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;
            ViewBag.DefaultMinBetValue = GetDefaultMinBetValue();

            var model = KenoService.GetSuperSetting(new Keno.KenoSetting.ViewModels.CustomerInformationViewModel
            {
                CustId = custId,
                SubAccId = 0,
                UserId = userId
            }).BetSettings.Where(o => !KenoSection.Instance.ExcludeBetTypeList.Contains(o.BetTypeId));

            model.ToList().ForEach(item =>
            {
                item.BetTypeName = Core.Services.Impl.CacheRef.GetBettype(item.BetTypeId);
            });

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/Keno/Content.cshtml", model);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "KenoBetting")]
        public ActionResult EditBetSettingPost(int custId, int uplineId, int userId, string txtCustomerObject)
        {
            var checkUmResult = serviceBase.CheckUM(custId, NonDcsProduct.RNGKeno, AppSession.CurrentSession.LangKey);
            if (!checkUmResult.IsSuccess)
            {
                return UmJsonResult(checkUmResult);
            }

            var customer = CustomerInfo(custId, userId);

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if (customer.RoleId.IsNotSuper())
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            var newSetting = JsonConvert.DeserializeObject<List<Keno.KenoSetting.Entities.BetSetting>>(txtCustomerObject, settings);
            var result = new List<IList<Core.Models.Result>>();

            foreach (var item in newSetting)
            {
                result.Add(KenoService.UpdateBetSettingSuper(item, custId, userId));
            }

            return JsonBase(result, settings);
        }
    }
}
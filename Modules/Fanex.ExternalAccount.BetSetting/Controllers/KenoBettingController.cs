namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Fanex.CustomerSetting.Module.BetSetting.Base;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.CustomerSetting.Security.Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class KenoBettingController : BettingControllerBase
    {
        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return this.ActionDenied(this.Request);
            }

            return this.PartialView("~/Areas/CustomerSetting/Views/BetSetting/Keno/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "KenoBetting")]
        public ActionResult Content(int id, int uplineId, int userId, string siteName)
        {
            var customer = this.CustomerInfo(id, userId);

            if (customer.RoleId.IsNotSuper())
            {
                return this.PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.SyncBingo = customer.SyncBingo;
            ViewBag.SyncLiveCasino = customer.SyncLiveCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            var model = this.KenoService.GetSuperSetting(new Keno.KenoSetting.ViewModels.CustomerInformationViewModel
            {
                CustId = id,
                SubAccId = 0,
                UserId = userId
            }).BetSettings.Where(o => !KenoSection.Instance.ExcludeBetTypeList.Contains(o.BetTypeId));

            model.ToList().ForEach(item =>
            {
                item.BetTypeName = Fanex.CustomerSetting.Module.Core.Services.Impl.CacheRef.GetBettype(item.BetTypeId);
            });

            return this.PartialView("~/Areas/CustomerSetting/Views/BetSetting/Keno/Content.cshtml", model);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "KenoBetting")]
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

            if (customer.RoleId.IsNotSuper())
            {
                return this.JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            var newSetting = JsonConvert.DeserializeObject<List<Keno.KenoSetting.Entities.BetSetting>>(txtCustomerObject, settings);
            var result = new List<IList<Core.Models.Result>>();

            foreach (var item in newSetting)
            {
                result.Add(this.KenoService.UpdateBetSettingSuper(item, id, userId));    
            }
            
            return this.JsonBase(result, settings);
        }
    }
}

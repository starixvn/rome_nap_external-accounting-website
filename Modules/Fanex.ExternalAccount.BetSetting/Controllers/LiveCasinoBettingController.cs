namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Base;
    using ViewModels;
    using Security.Authenticate;
    using Security.Configurations;
    using LiveCasinoSetting;
    using LiveCasinoSetting.Model;
    using LiveCasinoSetting.Model.Log;
    using LiveCasinoSetting.Request;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using RemotingRef;

    public class LiveCasinoBettingController : BettingControllerBase
    {
        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            return View("~/Areas/CustomerSetting/Views/BetSetting/LiveCasino/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "LiveCasinoBetting")]
        public ActionResult Content(int id, int uplineId, int userId, string siteName)
        {
            var customer = CustomerInfo(id, userId);

            if (customer.RoleId.IsNotSuper() || !customer.SyncLiveCasino)
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.SyncBingo = customer.SyncBingo;
            ViewBag.SyncLiveCasino = customer.SyncLiveCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            IMemberWinLimit winLimit = MemberWinLimitService.GetMemberWinLimit(id);
            List<GameType> bettingLimit = GameSettingService.GetByCustomerId(id);
            CustomerInfo customerInfo = SettingService.GetCustomerInfo(id);

            winLimit.IsDailyAutoReset = customerInfo.Limit.IsDailyAutoReset;
            winLimit.MaxWinLimit = customerInfo.Limit.MaxWinLimit;

            bettingLimit.ForEach(item =>
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en");
            });

            return View(
                "~/Areas/CustomerSetting/Views/BetSetting/LiveCasino/Content.cshtml",
                new LiveCasinoModel
                {
                    MemberWinLimit = winLimit as MemberWinLimit,
                    BettingLimits = bettingLimit
                });
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "LiveCasinoBetting")]
        public ActionResult EditBetSettingPost(int id, int uplineId, int userId, string txtCustomerObject, string siteName = "ACCUT")
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            Customer customer = CustomerInfo(id, userId);
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if (customer.RoleId.IsNotSuper() || !customer.SyncLiveCasino)
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            IRequestContext request = new RequestContext
            {
                CustId = id,
                IsEditSuper = true,
                Language = "En",
                SignUpIp = Request.UserHostAddress,
                UplineId = uplineId,
                UserId = userId
            };

            LiveCasinoModel model = JsonConvert.DeserializeObject<LiveCasinoModel>(txtCustomerObject);
            Result resultWinLimit = MemberWinLimitService.UpdateMemberWinLimit(model.MemberWinLimit, request);

            bool success = true;
            string message = string.Empty;

            if (model.BettingLimits != null && model.BettingLimits.Count > 0)
            {
                success = UpdateGameSetting(id, userId, model.BettingLimits);
            }

            if (!success)
            {
                return JsonBase(new { ErrorCode = "1", ErrorMessage = string.Join("<br />", resultWinLimit.ErrorMessage, message) }, settings);
            }

            return JsonBase(resultWinLimit, settings);
        }

        private bool UpdateGameSetting(int id, int userId, List<GameType> gameTypes)
        {
            InsertAccountRequest insertReq = new InsertAccountRequest { CustId = id, SuperId = id, UserId = userId, RoleId = Role.Super };
            Dictionary<int, Result> resultBettingLimit = GameSettingService.UpdateGameSetting(5, gameTypes, insertReq);

            bool success = true;
            string message = string.Empty;

            foreach (var item in resultBettingLimit)
            {
                if (!string.IsNullOrWhiteSpace(item.Value.ErrorCode) &&
                    item.Value.ErrorCode != ErrorCode.NOT_UPDATE_BETTING_LIMIT &&
                    item.Value.ErrorCode != ErrorCode.NOT_UPDATE_PT_SETTING)
                {
                    success = false;
                    message = string.Join("<br />", message, item.Value.ErrorMessage);
                }
            }

            List<GameType> oldSetting = GameSettingService.GetByCustomerId(id);
            LogService.WriteLog(new LogUpdateStakeLevel(oldSetting, gameTypes, resultBettingLimit)
            {
                Action = LogActions.UpdateStakeLevel_Super,
                ErrorCode = success ? string.Empty : "1",
                ErrorMessage = success ? string.Empty : message,
                RoleId = Role.Super,
                CustId = id,
                SignUpIp = Request.UserHostAddress,
                Language = "en",
                UserId = userId
            });

            return success;
        }
    }
}
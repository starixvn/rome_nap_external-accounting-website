namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using AutoMapper;
    using Base;
    using ExternalAccount.Core.Shared;
    using GoldDeluxe.CoreSetting.CommandResult;
    using GoldDeluxe.CoreSetting.Enumerations;
    using GoldDeluxe.CoreSetting.ParameterObjects;
    using GoldDeluxe.CoreSetting.Services;
    using GoldDeluxe.CoreSetting.ViewModels;
    using Security.Authenticate;
    using Security.Configurations;

    public class GoldDeluxeBettingController : BettingControllerBase
    {
        private readonly ISettingService _settingService;

        public GoldDeluxeBettingController()
            : this(null)
        {
        }

        public GoldDeluxeBettingController(ISettingService settingService)
        {
            _settingService = settingService ?? new SettingService();
        }

        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj =
                Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as
                    IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            return View("~/Areas/CustomerSetting/Views/BetSetting/GoldDeluxe/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "GoldDeluxeBetting")]
        public ActionResult UpdateGameSetting(EditSettingViewModel model)
        {
            var customer = CustomerInfo(model.CustId, model.ActorId);

            if (model.Level != CustomerLevel.Super || customer.HideGoldDeluxe)
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" });
            }

            var param = Mapper.Map<EditSettingViewModel, UpdateGameSettingParameters>(model);
            var updateResult = new MultipleCommandResults
            {
                CommandResultsList = new List<ICommandResults>
                {
                    _settingService.UpdateGameSetting(param)
                }
            };

            return Json(updateResult);
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "GoldDeluxeBetting")]
        public ActionResult Content(int custId,
            int upLineId,
            int level,
            int upLineLevel,
            int actorId,
            int subAccId = 0,
            string language = "E",
            FormType formType = FormType.ReadOnly)
        {
            var customer = CustomerInfo(custId, actorId);

            if (customer.RoleId.IsNotSuper() || customer.HideGoldDeluxe)
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.SyncBingo = customer.SyncBingo;
            ViewBag.SyncLiveCasino = customer.SyncLiveCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            var model = new EditSettingViewModel
            {
                CustId = custId,
                UpLineId = upLineId,
                Level = (CustomerLevel)level,
                UpLineLevel = (CustomerLevel)upLineLevel,
                ActorId = actorId,
                SubAccId = subAccId,
                Language = language,
                FormType = formType
            };

            var param = Mapper.Map<EditSettingViewModel, GetSettingParameters>(model);
            model.Setting = _settingService.GetSetting(param);

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/GoldDeluxe/Content.cshtml", model);
        }
    }
}
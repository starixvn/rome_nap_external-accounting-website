namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using AutoMapper;
    using ExternalAccount.Core.Shared;
    using Fanex.CustomerSetting.Module.BetSetting.Base;
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.CustomerSetting.Security.Configurations;
    using Fanex.GoldDeluxe.CoreSetting.CommandResult;
    using Fanex.GoldDeluxe.CoreSetting.Enumerations;
    using Fanex.GoldDeluxe.CoreSetting.ParameterObjects;
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using Fanex.GoldDeluxe.CoreSetting.ViewModels;
    using Fanex.GoldDeluxe.CoreSetting.Web.Filters;

    public class GoldDeluxeBettingController : BettingControllerBase
    {
        private const string EditGameSettingPagePath = "~/Areas/GoldDeluxeSetting/Views/EditSetting/EditGameSetting.cshtml";
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
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return this.ActionDenied(this.Request);
            }

            return this.View("~/Areas/CustomerSetting/Views/BetSetting/GoldDeluxe/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "GoldDeluxeBetting")]
        public ActionResult UpdateGameSetting(EditSettingViewModel model)
        {
            Customer customer = this.CustomerInfo(model.CustId, model.ActorId);

            if (model.Level != CustomerLevel.Super || customer.HideGoldDeluxe)
            {
                return this.JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" });
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
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
        public ActionResult Content(int custId,
            int upLineId,
            int level,
            int upLineLevel,
            int actorId,
            int subAccId = 0,
            string language = "E",
            FormType formType = FormType.ReadOnly)
        {
            Customer customer = this.CustomerInfo(custId, actorId);

            if (customer.RoleId.IsNotSuper() ||  customer.HideGoldDeluxe)
            {
                return this.PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
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

            return this.PartialView("~/Areas/CustomerSetting/Views/BetSetting/GoldDeluxe/Content.cshtml", model);
        }
    }
}
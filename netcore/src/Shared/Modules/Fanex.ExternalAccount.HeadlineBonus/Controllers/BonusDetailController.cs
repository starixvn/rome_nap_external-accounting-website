namespace Fanex.ExternalAccount.HeadlineBonus.Controllers
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.HeadlineBonus.Services;
    using Fanex.ExternalAccount.HeadlineBonus.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class BonusDetailController : BaseController
    {
        private readonly IBonusDetailService service;

        internal BonusDetailController(IBonusDetailService service, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.service = service;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.BonusDetail, Policy = AuthenticationType.Read)]
        public IActionResult Index()
        {
            var hasHostPermission = AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.BonusDetail, AuthenticationType.Host);

            return PartialViewBase(new BonusDetailViewModel(hasHostPermission));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.BonusDetail, Policy = AuthenticationType.Read)]
        public IActionResult Search(SearchBonusViewModel viewModel)
            => Json(service.GetBonusDetailReport(
                viewModel.GroupName,
                viewModel.Member,
                viewModel.FromDate,
                viewModel.ToDate,
                viewModel.StatusId,
                viewModel.IsComplained,
                viewModel.PageIndex + 1,
                viewModel.PageSize));

        [HttpPost, AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.BonusDetail, Policy = AuthenticationType.Host)]
        public IActionResult CancelPending(CancelPendingViewModel viewModel)
        {
            var result = viewModel.IsValid()
                ? service.CancelPending(viewModel.GroupName, viewModel.CustId, viewModel.TransId, CurrentSession.UserId, CurrentContext.Ip4Address)
                : GetBadRequestResult();

            return Json(result);
        }

        [HttpPost, AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.BonusDetail, Policy = AuthenticationType.Host)]
        public IActionResult AddNew(AddNewBonusViewModel viewModel)
        {
            var result = viewModel.IsValid()
                ? service.AddNew(viewModel.GroupName, viewModel.Usernames, viewModel.AmountValue, CurrentSession.UserId, CurrentContext.Ip4Address)
                : GetBadRequestResult();

            return Json(result);
        }

        private static Result GetBadRequestResult()
            => new Result((int)HttpStatusCode.BadRequest, "Bad Request");
    }
}

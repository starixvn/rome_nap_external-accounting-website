namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Fanex.ExternalAccount.Controllers.Requests.MarkAccountType;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Linq;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Read)]
    public class MarkAccountTypeController : BaseController
    {
        private readonly IMarkAccountTypeService markAccountTypeService;
        private const int DefaultPageSize = 50;

        public MarkAccountTypeController(
            IMarkAccountTypeService markAccountTypeService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            IAuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.markAccountTypeService = markAccountTypeService;
        }

        public IActionResult Index()
            => PartialViewBase();

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult Seach([FromBody] SearchMarkAccountRequest request)
        {
            var result = markAccountTypeService.Search(new SearchMarkAccountTypeParams {
                GroupId = request.GroupId,
                GroupName = request.GroupName,
                Level = request.Level,
                Type = request.Type,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize
            });
            
            return Ok(result);
        }


        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAccount()
        {
            return PartialViewBase();
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Add([FromBody] NewMarkAccountRequest request)
        {
            var result = markAccountTypeService.Add(new NewMarkAccountTypeParams
            {
                GroupName = request.GroupName,
                TypeId = request.TypeId,
                Usernames = request.Usernames,
                Remark = request.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });
            
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Update([FromBody] UpdateMarkAccountRequest request)
        {
            var result = markAccountTypeService.SingleUpdate(new UpdateMarkAccountTypeParams
            {
                CustId = request.CustId,
                TypeId = request.TypeId,
                Remark = request.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });
            
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMultiple([FromBody] UpdateMultipelMarkAccountRequest request)
        {
            var updateAccountTypes = request.UpdateAccounts.Select(account => new UpdateMarkAccountTypeParams {
                CustId = account.CustId,
                Username = account.Username,
                TypeId = account.TypeId,
                Remark = account.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });

            var result = markAccountTypeService.MultipleUpdate(updateAccountTypes);
            
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult Unmark([FromBody] UnmarkAccountRequest request)
        {
            var result = markAccountTypeService.Unmark(new UnmarkAccountTypeParams
            {
                CustId = request.CustId,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });
            
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult UnmarkAll([FromBody] UnmarkAccountRequest request)
        {
            var result = markAccountTypeService.UnmarkAll(new UnmarkAccountTypeParams
            {
                Accounts = request.Accounts,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });
            
            return Ok(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult MultipleEditAccountType([FromBody] UnmarkAccountRequest request)
        {
            return PartialViewBase();
        }

    }
}

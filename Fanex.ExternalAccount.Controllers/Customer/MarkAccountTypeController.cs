namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests.MarkAccountType;
    using Fanex.ExternalAccount.Core.Authorization;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Read)]
    public class MarkAccountTypeController : BaseController
    {
        private readonly IMarkAccountTypeService markAccountTypeService;
        private const int DefaultPageSize = 50;

        public MarkAccountTypeController() : this(null, null)
        {
        }

        public MarkAccountTypeController(IMarkAccountTypeService markAccountTypeService, AuthorizationService authorizationService)
        {
            this.markAccountTypeService = markAccountTypeService ?? new MarkAccountTypeService();
        }

        public ActionResult Index()
            => PartialViewBase();

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult Seach(SearchMarkAccountRequest request)
        {
            return Json(markAccountTypeService.Search(new SearchMarkAccountTypeParams {
                GroupId = request.GroupId,
                GroupName = request.GroupName,
                Level = request.Level,
                Type = request.Type,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize
            }));
        }


        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MarkAccount()
        {
            return PartialViewBase();
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult Add(NewMarkAccountRequest request)
        {
            return Json(markAccountTypeService.Add(new NewMarkAccountTypeParams
            {
                GroupName = request.GroupName,
                TypeId = request.TypeId,
                Usernames = request.Usernames,
                Remark = request.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            }));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult Update(UpdateMarkAccountRequest request)
        {
            return Json(markAccountTypeService.SingleUpdate(new UpdateMarkAccountTypeParams
            {
                CustId = request.CustId,
                TypeId = request.TypeId,
                Remark = request.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            }));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult UpdateMultiple(UpdateMultipelMarkAccountRequest request)
        {
            var updateAccountTypes = request.UpdateAccounts.Select(account => new UpdateMarkAccountTypeParams {
                CustId = account.CustId,
                Username = account.Username,
                TypeId = account.TypeId,
                Remark = account.Remark,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            });

            return Json(markAccountTypeService.MultipleUpdate(updateAccountTypes));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult Unmark(UnmarkAccountRequest request)
        {
            return Json(markAccountTypeService.Unmark(new UnmarkAccountTypeParams
            {
                CustId = request.CustId,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            }));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UnmarkAll(UnmarkAccountRequest request)
        {
            return Json(markAccountTypeService.UnmarkAll(new UnmarkAccountTypeParams
            {
                Accounts = request.Accounts,
                UserId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            }));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.MarkAccountType, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult MultipleEditAccountType(UnmarkAccountRequest request)
        {
            return PartialViewBase();
        }

    }
}

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using Fanex.Customer.Client.NETFramework;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Controllers.Authentication;
    using Fanex.ExternalAccount.Controllers.Requests.Customer;
    using Fanex.ExternalAccount.Controllers.ViewModels.Customer;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Service.TelegramBot;
    using IBCUtils;
    using PTEngine.BusinessObjects;
    using Service;
    using Service.Impl.Customer;
    using Utilities;
    using ViewModel;
    using ViewModel.Customer;
    using ViewModel.Customer.Suspend;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using CustomerClient = Fanex.Customer.Client;

    public partial class CustomerController : BaseController //DevSkim: ignore DS184626
    {
        private const string AssetsVersionCacheKey = "Customer:AssetsVersion";
        private const string HashKeySessionKey = "Fanex.ExternalAccount.Controllers.Customer.CustomerController.HashKey";
        private readonly string cdnUri = ConfigurationManager.AppSettings["Fanex.Customer:CdnUri"];
        private readonly int SystemId = int.Parse(ConfigurationManager.AppSettings["Fanex.Customer:SystemId"]);

        private const byte SystemCode_ExternalNapBetSetting = 5;
        private const byte SystemCode_ExternalNapBetSettingSpecialSite = 9;
        private const byte SystemCode_ExternalNapSpecialSite = 8;
        private const byte SystemCode_ExternalNap = 4;

        private readonly CustomerClient.Services.ICustomerService customerServiceClient = new CustomerClient.Services.CustomerService(new CustomerClient.Configurations.CustomerClientConfiguration().Initialize());

        private readonly CustomerService _customerService = new CustomerService();
        private readonly MasterService _masterService = new MasterService();
        private readonly SuperService _superService = new SuperService();
        private readonly IIPInformatonService ipInformationService = new IPInformationService();
        private readonly ITelegramBotService _telegramChatbotService = new TelegramBotService();
        private readonly ProductStatusService productStatusService = new ProductStatusServiceImpl();

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public async Task<ActionResult> CreateCustomer(int uplineId, byte uplineRoleId, string uplineUsername)
        {
            var limitedRulesResult = await customerServiceClient.CheckAllLimitedRules(uplineId, uplineRoleId, uplineUsername);
            if (limitedRulesResult.IsFailed)
            {
                return LimitationPage(limitedRulesResult.Message);
            }

            var viewModel = new CreateCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), SystemId),
                cdnUri,
                GetAssetsVersion(),
                GetHashKey(),
                uplineId,
                uplineRoleId,
                await GetSuggestUsername(uplineUsername, uplineRoleId));

            return PartialViewBase(viewModel, HttpContext.IsDebuggingEnabled ? "CreateCustomer-Debug" : "CreateCustomer");
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public async Task<ActionResult> EditCustomer(int custId, int roleId, bool isBetSettingOnly = false)
        {
            var currentSession = new Core.Context.CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, isBetSettingOnly);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), SystemId, systemCode),
                cdnUri,
                GetAssetsVersion());

            return PartialViewBase(editCustomerViewModel, HttpContext.IsDebuggingEnabled ? "EditCustomer-Debug" : "EditCustomer");
        }

        private static int GetSystemCode(bool isSpecialSite, bool isBetSettingOnly)
        {
            if (isSpecialSite)
            {
                return isBetSettingOnly ? SystemCode_ExternalNapBetSettingSpecialSite : SystemCode_ExternalNapSpecialSite;
            }


            return isBetSettingOnly ? SystemCode_ExternalNapBetSetting : SystemCode_ExternalNap;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public async Task<ActionResult> Create(CreateCustomerRequest request)
        {
            var generalInfo = request.GeneralInfo;
            var rawPassword = Encryptor.Decrypt(generalInfo.Password, HttpContext.Session[HashKeySessionKey].ToString());
            generalInfo.Password = Encryption.CfsEnCode(rawPassword);

            var response = await customerServiceClient.CreateCustomer(new CustomerClient.Params.CreateCustomerParam(
                generalInfo, request.ProductGroups, AppSession.AccId, 0, CurrentContext.Ip4Address, CustomerClient.Constants.CreationModes.FullMode));
            return Content(response);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Host)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public async Task<ActionResult> Update(UpdateCustomerRequest request)
        {
            var response = await customerServiceClient.UpdateCustomer(
                new CustomerClient.Params.UpdateCustomerParam(
                    request.GeneralInfo,
                    request.ProductGroups,
                    AppSession.AccId, 
                    0,
                    request.CustId,
                    "",
                    request.RoleId,
                    request.RoleId + 1,
                    CurrentContext.Ip4Address,
                    "en-US"));

            return Content(response);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public async Task<ActionResult> GetCustomerSetting(int custId, byte roleId, int systemId, string language = "en-US", bool isGetUpline = false)
            => Content(await customerServiceClient.GetCustomerSetting(custId, roleId, systemId, language, isGetUpline));

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public async Task<ActionResult> CheckExistedCustomer(string username)
            => Json(await customerServiceClient.CheckExistedCustomer(username), JsonRequestBehavior.AllowGet);

        [ExternalAccountAuthenticate(Function = eFunctions.CreateNew, Policy = AuthenticationType.Read)]
        private Task<string> GetSuggestUsername(string actorName, byte roleId)
            => customerServiceClient.GetSuggestUsername(actorName, roleId);

        private string GetApplicationPath()
        {
            var applicationPath = Response.ApplyAppPathModifier(Request.ApplicationPath);
            if (!applicationPath.EndsWith("/"))
            {
                applicationPath += "/";
            }

            return applicationPath;
        }

        private string GetHashKey()
        {
            var hashKey = CaptchaUtil.GetCaptchaString(4);
            HttpContext.Session.Add(HashKeySessionKey, hashKey);

            return hashKey;
        }

        private static string GetAssetsVersion()
        {
            ObjectCache cache = MemoryCache.Default;
            var version = cache.Get(AssetsVersionCacheKey) as string;
            if (string.IsNullOrWhiteSpace(version))
            {
                version = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                cache.Set(AssetsVersionCacheKey, version, DateTime.Now.AddHours(4));
            }
            return version;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult CustomerList() => PartialViewBase();

        public ActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            var custStatus = _customerService.GetCustomerAllStatus(custId, Config.SettingStatus);
            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            ViewBag.HideVirtualSports = custStatus.HideVirtualSport;
            var visibledProducts = GetVisibledProducts(id, custId);

            return PartialViewBase(visibledProducts);
        }

        private List<Product> GetVisibledProducts(int roleId, int custId)
        {
            var visibledProducts = new List<Product>();
            var productStatusList = productStatusService.GetProductStatusList(custId, roleId, Config.SettingStatus);

            foreach (var product in ProductSettings.SiteProducts.Values)
            {
                var productStatus = productStatusList.FirstOrDefault(p => p.ProductId == product.Id);
                if (productStatus != null
                    && !productStatus.IsHidden
                    && productStatus.IsSyncedUpline
                    && ProductSettings.IsShowProduct(product.Id)
                    && product.Id != ProductConstants.SabaVirtualSports)
                {
                    visibledProducts.Add(product);
                }
            }

            return visibledProducts;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public ActionResult ViewIP(string ip)
        {
            var ipInfo = new IPInfo { Flag = "-.gif", IP = ip };
            if (Utils.IsValidIpAddress(ip))
            {
                if (!AuthorizationService.CanViewVNIP(ip))
                {
                    return PartialViewBase("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
                }

                var info = ipInformationService.GetFromIP(ip);
                ipInfo = new IPInfo
                {
                    IP = ip,
                    City = string.IsNullOrWhiteSpace(info.IPCity) ? string.Empty : info.IPCity,
                    Country = string.IsNullOrWhiteSpace(info.CountryLong) ? string.Empty : info.CountryLong,
                    Flag = string.IsNullOrWhiteSpace(info.CountryShort) ? "-.gif" : info.CountryShort + ".gif",
                    Info = string.Format(ViewLang.IPNote, ip)
                };
            }

            return PartialViewBase(ipInfo);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = _superService.ConfigDoubleComm(
                new UpdateDoubleCommCriteria(
                    custid,
                    allow,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address));

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public ActionResult EditInfo(int custId)
        {
            var model = _customerService.UserInfo(custId, Config.SettingStatus);
            model.FirstName = HttpUtility.HtmlDecode(model.FirstName);
            model.LastName = HttpUtility.HtmlDecode(model.LastName);
            model.Phone = HttpUtility.HtmlDecode(model.Phone);
            model.MobilePhone = HttpUtility.HtmlDecode(model.MobilePhone);
            model.Email = HttpUtility.HtmlDecode(model.Email);
            model.Fax = HttpUtility.HtmlDecode(model.Fax);
            model.Remark = HttpUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditInfoPost(DTO.Customer.Customer customer)
        {
            var result = _customerService.EditUserInfo(customer, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm, Policy = AuthenticationType.Read)]
        public ActionResult ConfigMultipleDoubleComm(int id) => PartialViewBase(id);

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult TransferCondition()
        {
            var custId = int.Parse(Request["CustId"]);
            var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();

            return PartialViewBase(isSuperInAutomateTransfer);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public ActionResult EditTransferCondition(int custId, int roleId)
        {
            ViewBag.CustId = custId;
            var model = _masterService.GetTransferOption(custId);
            var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();

            return PartialViewBase(new TransferConditionViewModel(model, isSuperInAutomateTransfer, roleId));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult EditTransferConditionPost(string transferOption, int autoTransferCondition, int custId)
        {
            var result = _masterService.EditTransferOption(transferOption, autoTransferCondition, custId, CurrentSession.CustId,
                CurrentContext.Ip4Address, 0, "E");

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public ActionResult ResetPassword(int custId)
        {
            var user = _customerService.UserInfo(custId, Config.SettingStatus);
            ViewBag.SCode = Encryptor.RandomCaptcha;
            return PartialViewBase(user);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdatePassword(int custId, string password, byte roleId)
        {
            var rawPassword = Encryptor.Decrypt(password, Encryptor.RandomCaptcha);
            var result = _customerService.ResetPassword(
                new ResetPasswordCriteria(
                    custId,
                    rawPassword,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address)
                );

            if (result.ErrorCode.Equals("0"))
            {
                if (roleId != (byte)CustomerRole.Member)
                {
                    Task.Run(() => _telegramChatbotService.NotifyChangePassword(custId, 0));
                }

                _customerService.KickoutCustomer(custId, roleId, CurrentContext.Ip4Address);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        public ActionResult AccessDenied() => ActionDenied(Request);

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult UnsuspendSingle(int custId, string userName)
        {
            return PartialViewBase(_customerService.GetSingleUpdateSuspendViewModel(custId, userName));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult MultipleSuspend(string custIdList, int roleId)
        {
            return PartialViewBase(new MultipleUpdateViewModel(roleId, _customerService.GetMultipleSuspendCustomerViewModel(new GetMultipleSuspendCustomerInfoCriteria(custIdList, roleId))));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetSuspendDownlinesDetail(int custId, bool isAffected, int downlineRoleId, int pageSize = 50, int pageIndex = 0)
        {
            return Json(_customerService.GetSuspendDownlinesDetail(new GetSuspendCustomerDetailInfoCriteria(custId, isAffected, downlineRoleId, pageSize, pageIndex)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public JsonResult UpdateSuspendSingle(int custId, bool suspended)
        {
            return Json(_customerService.SuspendCustomer(new UpdateSuspendCustomerCriteria(CurrentSession.UserId, custId, suspended, "E", CurrentContext.Ip4Address)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        public JsonResult UpdateSuspendMultiple(
            List<MultipleUpdateSuspendRequest> request,
            string groupName,
            int roleId)
        {
            var custIds = request.Select(item => item.CustId);
            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return Json(new DTO.Result(1, "You are not authorized on this function!"));
            }

            return Json(_customerService.SuspendMultipleCustomer(request, CurrentSession.UserId, CurrentContext.Ip4Address, roleId));
        }

        private ActionResult LimitationPage(string message)
            => PartialView("~/Views/Customer/LimitMemberCreation.cshtml", message);
    }
}
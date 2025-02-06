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
    using Fanex.Customer.Client;
    using Fanex.ExternalAccount.Controllers.Authentication;
    using Fanex.ExternalAccount.Controllers.Requests.Customer;
    using Fanex.ExternalAccount.Controllers.ViewModels.Customer;
    using Fanex.ExternalAccount.DTO.Products;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Service.TelegramBot;
    using IBCUtils;
    using Service;
    using Service.Impl.Customer;
    using Utilities;
    using ViewModel;
    using ViewModel.Customer;
    using ViewModel.Customer.Suspend;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;

    using CustomerClient = Fanex.Customer.Client;

    public partial class CustomerController : BaseController
    {
        private const string AssetsVersionCacheKey = "Customer:AssetsVersion";
        private const string HashKeySessionKey = "Fanex.ExternalAccount.Controllers.Customer.CustomerController.HashKey";
        private readonly string cdnUri = ConfigurationManager.AppSettings["Fanex.Customer:CdnUri"];

        private const byte SystemCode_ExternalNapBetSetting = 5;
        private const byte SystemCode_ExternalNapBetSettingSpecialSite = 9;
        private const byte SystemCode_ExternalNapSpecialSite = 8;
        private const byte SystemCode_ExternalNap = 4;

        private readonly CustomerClient.Services.ICustomerService customerServiceClient = new CustomerClient.Services.CustomerService();

        private readonly CustomerService _customerService = new CustomerService();
        private readonly MasterService _masterService = new MasterService();
        private readonly SuperService _superService = new SuperService();
        private readonly IIPInformatonService ipInformationService = new IPInformationService();
        private readonly ITelegramBotService _telegramChatbotService = new TelegramBotService();
        private readonly ProductStatusService productStatusService = new ProductStatusServiceImpl();
        private readonly AuthorizationService authorizationService = new AuthorizationServiceImpl();
        private readonly ISiteDBConfigService siteDBConfigService = new SiteDBConfigService();

        [ExternalCustomerAuthFilter(Function = eFunctions.CreateNew)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Transfer)]
        public async Task<ActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            var limitedRulesResult = await customerServiceClient.CheckAllLimitedRules(request.UplineId, request.UplineRoleId, request.UplineUsername);
            if (limitedRulesResult.IsFailed)
            {
                return LimitationPage(limitedRulesResult.Message);
            }

            request.SuggestUsername = await GetSuggestUsername(request.UplineUsername, request.UplineRoleId);

            var userId = CurrentSession.UserId;
            request.NoPermissionEditCredit = !AuthorizationService.HasUpdateCreditPermission(userId);

            var viewModel = new CreateCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), CustomerClientManager.ClientConfigs.SystemId),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson(),
                GetHashKey(),
                BuildPageTitle(request.UplineRoleId)
              );

            return PartialViewBase(viewModel, HttpContext.IsDebuggingEnabled ? "CreateCustomer-Debug" : "CreateCustomer");
        }

        private static string BuildPageTitle(byte uplineRoleId)
        {
            switch (uplineRoleId)
            {
                case Role.Super:
                    return "Add New Master";

                case Role.Master:
                    return "Add New Agent";

                case Role.Agent:
                    return "Add New Member";

                default:
                    return "Add New Super";
            }
        }

        [ExternalCustomerAuthFilter]
        public ActionResult EditCustomer(EditCustomerRequest request)
        {
            var currentSession = new CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, false);
            var userId = CurrentSession.UserId;
            request.NoPermissionEditCredit = !AuthorizationService.HasUpdateCreditPermission(userId);

            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel, HttpContext.IsDebuggingEnabled ? "EditCustomer-Debug" : "EditCustomer");
        }

        [ExternalCustomerAuthFilter(IsBettingLimit = true)]
        public ActionResult EditSuperBetSetting(EditCustomerRequest request)
        {
            var currentSession = new CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, true);

            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel, HttpContext.IsDebuggingEnabled ? "EditCustomer-Debug" : "EditCustomer");
        }

        [HttpPost]
        [AntiXSRFFilter]
        [ExternalCustomerAuthFilter(IsBettingLimit = true, IsHost = true)]
        public async Task<ActionResult> EditSuperBetSetting(UpdateCustomerRequest request)
        {
            var response = await customerServiceClient.UpdateCustomer(
                new CustomerClient.Params.UpdateCustomerParam(
                    null,
                    request.ProductGroups,
                    CurrentSession.UserId,
                    0,
                    request.CustId,
                    "",
                    request.RoleId,
                    request.UplineRoleId,
                    CurrentContext.Ip4Address,
                    siteDBConfigService.GetSiteDBConfigByCustId(request.CustId).CustDBGroupID,
                    request.Site,
                    request.SiteLabel,
                    request.Group,
                    null));

            return Content(response);
        }

        [ExternalCustomerAuthFilter(IsBettingLimit = true)]
        public ActionResult EditMultipleSuperBetSetting(EditMultipleSuperBetSettingRequest request)
        {
            if (Role.IsNotSuper(request.RoleId)
                || !authorizationService.IsBelong(CurrentSession.CurrencyGroup, request.CustIds.Split(',').Select(Int32.Parse).ToList()))
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            var currentSession = new CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, true);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel, HttpContext.IsDebuggingEnabled ? "EditMultiple-Debug" : "EditMultiple");
        }

        [HttpPost]
        [AntiXSRFFilter]
        [ExternalCustomerAuthFilter(IsHost = true, IsBettingLimit = true)]
        public async Task<ActionResult> EditMultipleSuperBetSetting(UpdateMultipleCustomerCommand request)
        {
            if (Role.IsNotSuper(request.RoleId)
                || !authorizationService.IsBelong(CurrentSession.CurrencyGroup, request.CustomerIds.Split(',').Select(Int32.Parse).ToList()))
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            var response = await customerServiceClient.UpdateMultipleCustomers(
            new CustomerClient.Params.UpdateMultipleCustomerParam(
                request.ProductGroups,
                CurrentSession.UserId,
                0,
                request.CustId,
                "",
                request.RoleId,
                request.UplineRoleId,
                CurrentContext.Ip4Address,
                siteDBConfigService.GetSiteDBConfigByCustId(request.CustId).CustDBGroupID,
                request.CustomerIds,
                request.CustomerNames,
                request.Site,
                request.SiteLabel,
                request.Group,
                null));

            return Content(response);
        }

        private static int GetSystemCode(bool isSpecialSite, bool isBetSettingOnly)
        {
            if (isSpecialSite)
            {
                return isBetSettingOnly ? SystemCode_ExternalNapBetSettingSpecialSite : SystemCode_ExternalNapSpecialSite;
            }

            return isBetSettingOnly ? SystemCode_ExternalNapBetSetting : SystemCode_ExternalNap;
        }

        [HttpPost]
        [ExternalCustomerAuthFilter(Function = eFunctions.CreateNew, IsHost = true)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook, IsHost = true)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Transfer, IsHost = true)]
        [AntiXSRFFilter]
        public async Task<ActionResult> Create(CreateCustomerCommand request)
        {
            var generalInfo = request.GeneralInfo;
            var rawPassword = Encryptor.Decrypt(generalInfo.Password, HttpContext.Session[HashKeySessionKey].ToString());
            generalInfo.Password = rawPassword;

            var response = await customerServiceClient.CreateCustomer(new CustomerClient.Params.CreateCustomerParam(
                generalInfo,
                request.ProductGroups,
                CurrentSession.UserId,
                0,
                CurrentContext.Ip4Address,
                generalInfo.Site,
                generalInfo.SiteLabel,
                siteDBConfigService.GetSiteDBConfigBySite(generalInfo.Site).CustDBGroupID,
                request.Group,
                1,
                CustomerClient.Constants.CreationModes.FullMode));
            return Content(response);
        }

        [HttpPost]
        [ExternalCustomerAuthFilter(IsHost = true)]
        [AntiXSRFFilter]
        public async Task<ActionResult> Update(UpdateCustomerRequest request)
        {
            var response = await customerServiceClient.UpdateCustomer(
                new CustomerClient.Params.UpdateCustomerParam(
                    request.GeneralInfo,
                    request.ProductGroups,
                    CurrentSession.UserId,
                    0,
                    request.CustId,
                    "",
                    request.RoleId,
                    request.UplineRoleId,
                    CurrentContext.Ip4Address,
                    siteDBConfigService.GetSiteDBConfigByCustId(request.CustId).CustDBGroupID,
                    request.Site,
                    request.SiteLabel,
                    request.Group,
                    request.AgentId));

            return Content(response);
        }

        [HttpPost]
        [AntiXSRFFilter]
        [ExternalCustomerAuthFilter(IsHost = true)]
        public async Task<ActionResult> UpdateMultipleCustomers(UpdateMultipleCustomerCommand request)
        {
            var response = await customerServiceClient.UpdateMultipleCustomers(
                new CustomerClient.Params.UpdateMultipleCustomerParam(
                    request.ProductGroups,
                    CurrentSession.UserId,
                    0,
                    request.CustId,
                    "",
                    request.RoleId,
                    request.UplineRoleId,
                    CurrentContext.Ip4Address,
                    siteDBConfigService.GetSiteDBConfigByCustId(request.CustId).CustDBGroupID,
                    request.CustomerIds,
                    request.CustomerNames,
                    request.Site,
                    request.SiteLabel,
                    request.Group,
                    request.AgentId));

            return Content(response);
        }

        [ExternalCustomerAuthFilter]
        public ActionResult EditMultiple(EditMultipleCustomerRequest request)
        {
            var currentSession = new CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, false);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel, HttpContext.IsDebuggingEnabled ? "EditMultiple-Debug" : "EditMultiple");
        }

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public ActionResult SportBookInformation(SportBookInformationRequest request)
        {
            var currentSession = new CurrentSession();
            var systemCode = GetSystemCode(currentSession.IsBelongToSpecialGroups, false);
            var appPath = GetApplicationPath();
            request.ApplicationPath = appPath;
            var viewModel = new SportBookInformationViewModel(
                new AppConfiguration(appPath, systemCode),
                cdnUri,
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(viewModel, HttpContext.IsDebuggingEnabled ? "SportBookInformation-Debug" : "SportBookInformation");
        }

        public async Task<ActionResult> GetCustomerSetting(GetCustomerSettingRequest request)
        {
            if (!authorizationService.HasReadPermissionOnFunctions(new List<string> { eFunctions.Sportsbook, eFunctions.SportsbookBetting, eFunctions.LiveAndRNGCasino, eFunctions.LiveAndRNGCasinoBetting }, CurrentSession.UserId))
            {
                return Redirect(CustomerClientManager.ClientConfigs.AccessDeniedUrl);
            }

            return Content(await customerServiceClient.GetCustomerSetting(request.CustId, request.RoleId, request.SystemId, request.UplineRoleId, request.Language, request.IsGetUpLine, request.ProductGroupId, request.SiteLabel, request.IsMultipleMode, request.GroupName, request.IsCopy, request.AgentId));
        }

        public async Task<ActionResult> GetCurrentWinLossAmount(int custId, int sportType, int betType, string language = "en-US")
            => Content(await customerServiceClient.GetCustomerCurrentWinLossAmounts(custId, sportType, betType, language));

        [ExternalCustomerAuthFilter(Function = eFunctions.CustomerList)]
        public async Task<ActionResult> CheckExistedCustomer(string username)
            => Json(await customerServiceClient.CheckExistedCustomer(username), JsonRequestBehavior.AllowGet);

        [ExternalCustomerAuthFilter(Function = eFunctions.CreateNew)]
        private Task<string> GetSuggestUsername(string actorName, byte roleId)
            => customerServiceClient.GetSuggestUsername(actorName, roleId);

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetMaxPayoutSettingInformation(string site, string siteLabel, string language = "en-US")
            => customerServiceClient.GetMaxPayoutSettingInformation(site, siteLabel, language);

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetPTSettingInformation(int? custId, string site, string language = "en-US")
            => customerServiceClient.GetPTSettingInformation(custId, false, site, language);

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetCommissionHint(int? custId, string site, string language = "en-US")
            => customerServiceClient.GetCommissionHint(custId ?? 0, site, language);

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetProductCategories(int? custId, string site, string siteLabel, int currency, string language = "en-US")
            => customerServiceClient.GetProductCategories(custId, site, siteLabel, currency, language);

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetWinLossLimitInformation(int? custId, string site, string siteLabel, int currency, string language = "en-US")
            => customerServiceClient.GetWinLossLimitInformation(custId, site, siteLabel, currency, language);

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
            ViewBag.CurrentLevel = id;
            ViewBag.UserId = custId;
            ViewBag.UplineId = uplineId;
            var productStatusList = productStatusService.GetProductStatusList(custId, id, Config.SettingStatus);
            var visibledProductGroups = GetVisibleProductGroups(productStatusList).Where(p => p.Id != ProductGroupConstants.Sportsbook);
            var visibledProducts = GetVisibledProducts(productStatusList);

            return PartialViewBase(new CustomerSettingsViewModel(visibledProductGroups, visibledProducts));
        }

        private static IEnumerable<Product> GetVisibleProductGroups(IList<ProductStatusDto> productStatusList)
        {
            var visibledProductGroups = productStatusList
                .Where(p => p.GroupId != null && p.GroupId != ProductGroupConstants.Sportsbook && !p.IsHidden)
                .Select(p => p.GroupId.Value)
                .Distinct()
                .Select(groupId => new Product(groupId, groupId, CacheRef.GetProductGroupName(groupId)));

            return visibledProductGroups;
        }

        private static IEnumerable<Product> GetVisibledProducts(IList<ProductStatusDto> productStatusList)
        {
            var visibledProducts = new List<Product>();

            foreach (var product in ProductSettings.SiteProducts.Values)
            {
                var productStatus = productStatusList.FirstOrDefault(p => p.ProductId == product.Id && p.GroupId == null);
                if (productStatus != null
                    && !productStatus.IsHidden
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
                if (roleId != Role.Member)
                {
                    Task.Run(() => _telegramChatbotService.NotifyChangePassword(custId, 0));
                }

                _customerService.KickoutCustomer(custId, roleId, CurrentContext.Ip4Address);
            }

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        public ActionResult AccessDenied() => ActionDenied(Request);

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        public ActionResult UnsuspendSingle(int custId, string userName)
        {
            return PartialViewBase(_customerService.GetSingleUpdateSuspendViewModel(custId, userName));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        public ActionResult MultipleSuspend(string custIdList, int roleId)
        {
            return PartialViewBase(new MultipleUpdateViewModel(roleId, _customerService.GetMultipleSuspendCustomerViewModel(new GetMultipleSuspendCustomerInfoCriteria(custIdList, roleId))));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetSuspendDownlinesDetail(int custId, bool isAffected, int downlineRoleId, int pageSize = 50, int pageIndex = 0)
        {
            return Json(_customerService.GetSuspendDownlinesDetail(new GetSuspendCustomerDetailInfoCriteria(custId, isAffected, downlineRoleId, pageSize, pageIndex)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Host)]
        public JsonResult UpdateSuspendSingle(int custId, bool suspended)
        {
            return Json(_customerService.SuspendCustomer(new UpdateSuspendCustomerCriteria(CurrentSession.UserId, custId, suspended, "E", CurrentContext.Ip4Address)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Host)]
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
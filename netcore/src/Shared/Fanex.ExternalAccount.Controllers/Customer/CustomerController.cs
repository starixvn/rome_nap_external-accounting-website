namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Caching.Memory;
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
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

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

        public CustomerController(
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            AuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        private string GetCdnUri() => _configuration?["Fanex.Customer:CdnUri"] ?? string.Empty;

        [ExternalCustomerAuthFilter(Function = eFunctions.CreateNew)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        [ExternalCustomerAuthFilter(Function = eFunctions.Transfer)]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
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
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson(),
                GetHashKey(),
                BuildPageTitle(request.UplineRoleId)
              );

            return PartialViewBase(viewModel);
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
        public async Task<IActionResult> EditCustomer(EditCustomerRequest request)
        {
            var systemCode = GetSystemCode(CurrentSession.IsBelongToSpecialGroups, false);
            var userId = CurrentSession.UserId;
            request.NoPermissionEditCredit = !AuthorizationService.HasUpdateCreditPermission(userId);
            request.ServiceType = await customerServiceClient.GetCashServiceStatus(request.CustId);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel);
        }

        [ExternalCustomerAuthFilter(IsBettingLimit = true)]
        public IActionResult EditSuperBetSetting(EditCustomerRequest request)
        {
            var systemCode = GetSystemCode(CurrentSession.IsBelongToSpecialGroups, true);

            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExternalCustomerAuthFilter(IsBettingLimit = true, IsHost = true)]
        public async Task<IActionResult> EditSuperBetSetting(UpdateCustomerRequest request)
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
        public IActionResult EditMultipleSuperBetSetting(EditMultipleSuperBetSettingRequest request)
        {
            if (Role.IsNotSuper(request.RoleId)
                || !authorizationService.IsBelong(CurrentSession.CurrencyGroup, request.CustIds.Split(',').Select(Int32.Parse).ToList()))
            {
                return JsonBase(new DTO.Result(1, "Access denied"));
            }

            var systemCode = GetSystemCode(CurrentSession.IsBelongToSpecialGroups, true);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExternalCustomerAuthFilter(IsHost = true, IsBettingLimit = true)]
        public async Task<IActionResult> EditMultipleSuperBetSetting(UpdateMultipleCustomerCommand request)
        {
            if (Role.IsNotSuper(request.RoleId)
                || !authorizationService.IsBelong(CurrentSession.CurrencyGroup, request.CustomerIds.Split(',').Select(Int32.Parse).ToList()))
            {
                return JsonBase(new DTO.Result(1, "Access denied"));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerCommand request)
        {
            var generalInfo = request.GeneralInfo;
            var sessionValue = HttpContext.Session.GetString(HashKeySessionKey);
            var rawPassword = Encryptor.Decrypt(generalInfo.Password, sessionValue);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCustomerRequest request)
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
        [ValidateAntiForgeryToken]
        [ExternalCustomerAuthFilter(IsHost = true)]
        public async Task<IActionResult> UpdateMultipleCustomers(UpdateMultipleCustomerCommand request)
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
        public async Task<IActionResult> EditMultiple(EditMultipleCustomerRequest request)
        {
            var systemCode = GetSystemCode(CurrentSession.IsBelongToSpecialGroups, false);
            int firstCustId = 0;
            if (!string.IsNullOrWhiteSpace(request.CustIds))
            {
                var firstIdStr = request.CustIds.Split(',').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "0";
                int.TryParse(firstIdStr, out firstCustId);
            }
            request.ServiceType = await customerServiceClient.GetCashServiceStatus(firstCustId);
            var editCustomerViewModel = new EditCustomerViewModel(
                new AppConfiguration(GetApplicationPath(), systemCode),
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(editCustomerViewModel);
        }

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public IActionResult SportBookInformation(SportBookInformationRequest request)
        {
            var systemCode = GetSystemCode(CurrentSession.IsBelongToSpecialGroups, false);
            var appPath = GetApplicationPath();
            request.ApplicationPath = appPath;
            var viewModel = new SportBookInformationViewModel(
                new AppConfiguration(appPath, systemCode),
                GetCdnUri(),
                GetAssetsVersion(),
                request.ToJson());

            return PartialViewBase(viewModel);
        }

        public async Task<IActionResult> GetCustomerSetting(GetCustomerSettingRequest request)
        {
            if (!authorizationService.HasReadPermissionOnFunctions(new List<string> { eFunctions.Sportsbook, eFunctions.SportsbookBetting, eFunctions.LiveAndRNGCasino, eFunctions.LiveAndRNGCasinoBetting }, CurrentSession.UserId))
            {
                return Redirect(CustomerClientManager.ClientConfigs.AccessDeniedUrl);
            }

            return Content(await customerServiceClient.GetCustomerSetting(
                request.CustId, 
                request.RoleId, 
                request.SystemId, 
                request.UplineRoleId, 
                request.Language, 
                request.IsGetUpLine, 
                request.ProductGroupId, 
                request.SiteLabel, 
                request.IsMultipleMode, 
                request.GroupName, 
                request.IsCopy, 
                request.AgentId));
        }

        public async Task<IActionResult> GetCashCustomerSetting(GetCustomerSettingRequest request)
        {
            if (!authorizationService.HasReadPermissionOnFunctions(new List<string> { eFunctions.Sportsbook, eFunctions.SportsbookBetting, eFunctions.LiveAndRNGCasino, eFunctions.LiveAndRNGCasinoBetting }, CurrentSession.UserId))
            {
                return Redirect(CustomerClientManager.ClientConfigs.AccessDeniedUrl);
            }

            if (!AppSettings.IsOfferedCashService(CurrentSession.CurrencyGroup))
            {
                return Redirect(CustomerClientManager.ClientConfigs.AccessDeniedUrl);
            }

            return Content(await customerServiceClient.GetCashCustomerSetting(
                request.CustId,
                request.RoleId,
                request.SystemId,
                request.UplineRoleId,
                request.Language,
                request.IsGetUpLine,
                request.ProductGroupId,
                request.SiteLabel));
        }

        public async Task<IActionResult> GetCashServiceStatus(GetCustomerSettingRequest request)
            => JsonBase(await customerServiceClient.GetCashServiceStatus(request.CustId));

        public async Task<IActionResult> GetCurrentWinLossAmount(int custId, int sportType, int betType, string language = "en-US")
            => Content(await customerServiceClient.GetCustomerCurrentWinLossAmounts(custId, sportType, betType, language));

        public async Task<IActionResult> GetCurrentWinAmount(int custId, int sportType, int betType, string language = "en-US")
            => Content(await customerServiceClient.GetCurrentWinAmounts(custId, sportType, betType, language));

        [ExternalCustomerAuthFilter(Function = eFunctions.CustomerList)]
        public async Task<IActionResult> CheckExistedCustomer(string username)
            => JsonBase(await customerServiceClient.CheckExistedCustomer(username));

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

        [ExternalCustomerAuthFilter(Function = eFunctions.Sportsbook)]
        public Task<string> GetBetLimitInformation(string site, string siteLabel, string language = "en-US")
            => customerServiceClient.GetBetLimitInformation(site, siteLabel, language);

        private string GetApplicationPath()
        {
            var pathBase = HttpContext.Request.PathBase.ToString();
            if (!pathBase.EndsWith("/"))
            {
                pathBase += "/";
            }
            return pathBase;
        }

        private string GetHashKey()
        {
            var hashKey = CaptchaUtil.GetCaptchaString(4);
            HttpContext.Session.SetString(HashKeySessionKey, hashKey);
            return hashKey;
        }

        private string GetAssetsVersion()
        {
            if (_memoryCache.TryGetValue(AssetsVersionCacheKey, out string version))
            {
                return version;
            }

            version = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(4));
            _memoryCache.Set(AssetsVersionCacheKey, version, cacheOptions);
            return version;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult CustomerList() => PartialViewBase();

        public IActionResult Settings(int id, int custId, int uplineId, string userName, string groupName)
        {
            var productStatusList = productStatusService.GetProductStatusList(custId, id, Config.SettingStatus);
            var visibledProductGroups = GetVisibleProductGroups(productStatusList).Where(p => p.Id != ProductGroupConstants.Sportsbook);
            var visibledProducts = GetVisibledProducts(productStatusList);

            var viewModel = new CustomerSettingsViewModel(visibledProductGroups, visibledProducts);
            return PartialViewBase(viewModel);
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
        public IActionResult ViewIP(string ip)
        {
            var ipInfo = new IPInfo { Flag = "-.gif", IP = ip };
            if (Utils.IsValidIpAddress(ip))
            {
                if (!AuthorizationService.CanViewVNIP(ip))
                {
                    return ActionDenied();
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
        [ValidateAntiForgeryToken]
        public IActionResult ConfigDoubleComm(int custid, bool allow)
        {
            var result = _superService.ConfigDoubleComm(
                new UpdateDoubleCommCommand(
                    custid,
                    allow,
                    CurrentSession.UserId,
                    CurrentContext.Ip4Address));

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public IActionResult EditInfo(int custId)
        {
            var model = _customerService.UserInfo(custId, Config.SettingStatus);
            model.FirstName = WebUtility.HtmlDecode(model.FirstName);
            model.LastName = WebUtility.HtmlDecode(model.LastName);
            model.Phone = WebUtility.HtmlDecode(model.Phone);
            model.MobilePhone = WebUtility.HtmlDecode(model.MobilePhone);
            model.Email = WebUtility.HtmlDecode(model.Email);
            model.Fax = WebUtility.HtmlDecode(model.Fax);
            model.Remark = WebUtility.HtmlDecode(model.Remark);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult EditInfoPost(DTO.Customer.Customer customer)
        {
            customer.FirstName = WebUtility.HtmlEncode(customer.FirstName);
            customer.LastName = WebUtility.HtmlEncode(customer.LastName);
            customer.Phone = WebUtility.HtmlEncode(customer.Phone);
            customer.MobilePhone = WebUtility.HtmlEncode(customer.MobilePhone);
            customer.Email = WebUtility.HtmlEncode(customer.Email);
            customer.Fax = WebUtility.HtmlEncode(customer.Fax);
            customer.Remark = WebUtility.HtmlEncode(customer.Remark);
            var result = _customerService.EditUserInfo(customer, CurrentContext.Ip4Address, CurrentSession.CustId, 0, "E");
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public IActionResult GetCustomerInfo(int custId)
        {
            var model = _customerService.UserInfo(custId, Config.SettingStatus);
            model.FirstName = WebUtility.HtmlDecode(model.FirstName);
            model.LastName = WebUtility.HtmlDecode(model.LastName);
            model.Phone = WebUtility.HtmlDecode(model.Phone);
            model.MobilePhone = WebUtility.HtmlDecode(model.MobilePhone);
            model.Email = WebUtility.HtmlDecode(model.Email);
            model.Fax = WebUtility.HtmlDecode(model.Fax);
            model.Remark = WebUtility.HtmlDecode(model.Remark);
            return JsonBase(model);
        }
        
        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public IActionResult EditInfoCashMember(int custId)
        {
            var model = new EditInfoCashMemberViewModel(
                GetAssetsVersion(),
                custId,
                "",
                true,
                GetApplicationPath(),
                GetCdnUri(),
                CustomerClientManager.ClientConfigs.SystemId);

            return PartialViewBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public async Task<string> GetCustomerBankInfo(int custId, int roleId)
            => await customerServiceClient.GetCustomerBankInfo(custId, roleId);
        
        [ExternalAccountAuthenticate(Function = eFunctions.EnableDoubleComm, Policy = AuthenticationType.Read)]
        public IActionResult ConfigMultipleDoubleComm(int id) => PartialViewBase(id);

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public IActionResult TransferCondition()
        {
            var custIdStr = HttpContext.Request.Query["CustId"].ToString();
            if (int.TryParse(custIdStr, out int custId))
            {
                var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();
                return PartialViewBase(isSuperInAutomateTransfer);
            }
            return BadRequest();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Read)]
        public IActionResult EditTransferCondition(int custId, int roleId)
        {
            var model = _masterService.GetTransferOption(custId);
            var isSuperInAutomateTransfer = _masterService.GetSuperInAutomateTransfer(custId).Any();

            return PartialViewBase(new TransferConditionViewModel(model, isSuperInAutomateTransfer, roleId));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Transfer, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult EditTransferConditionPost(string transferOption, int autoTransferCondition, int custId)
        {
            var result = _masterService.EditTransferOption(transferOption, autoTransferCondition, custId, CurrentSession.CustId,
                CurrentContext.Ip4Address, 0, "E");

            return JsonBase(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Read)]
        public IActionResult ResetPassword(int custId)
        {
            var user = _customerService.UserInfo(custId, Config.SettingStatus);
            return PartialViewBase(user);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EditCustomer, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(int custId, string password, byte roleId)
        {
            var rawPassword = Encryptor.Decrypt(password, Encryptor.RandomCaptcha);
            var result = _customerService.ResetPassword(
                new ResetPasswordCommand(
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

        public IActionResult AccessDenied() => ActionDenied();

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        public IActionResult UnsuspendSingle(int custId, string userName)
        {
            return PartialViewBase(_customerService.GetSingleUpdateSuspendViewModel(custId, userName));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        public IActionResult MultipleSuspend(string custIdList, int roleId)
        {
            return PartialViewBase(new MultipleUpdateViewModel(roleId, _customerService.GetMultipleSuspendCustomerViewModel(new GetMultipleSuspendCustomerInfoCriteria(custIdList, roleId))));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult GetSuspendDownlinesDetail(int custId, bool isAffected, int downlineRoleId, int pageSize = 50, int pageIndex = 0)
        {
            return JsonBase(_customerService.GetSuspendDownlinesDetail(new GetSuspendCustomerDetailInfoCriteria(custId, isAffected, downlineRoleId, pageSize, pageIndex)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Host)]
        public IActionResult UpdateSuspendSingle(int custId, bool suspended)
        {
            return JsonBase(_customerService.SuspendCustomer(new UpdateSuspendCustomerCommand(CurrentSession.UserId, custId, suspended, "E", CurrentContext.Ip4Address)));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticationType.Host)]
        public IActionResult UpdateSuspendMultiple(
            List<MultipleUpdateSuspendRequest> request,
            string groupName,
            int roleId)
        {
            var custIds = request.Select(item => item.CustId);
            if (!AuthorizationService.IsBelong(groupName, custIds))
            {
                return JsonBase(new DTO.Result(1, "You are not authorized on this function!"));
            }

            return JsonBase(_customerService.SuspendMultipleCustomer(request, CurrentSession.UserId, CurrentContext.Ip4Address, roleId));
        }

        private IActionResult LimitationPage(string message)
            => PartialViewBase(message);
    }
}
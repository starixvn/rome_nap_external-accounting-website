namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.ExternalAccount.Controllers.Requests.CustomerList;
    using Fanex.ExternalAccount.Core.Authorization;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Excel.Customer.CustomerListPage;
    using Fanex.ExternalAccount.Service.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Customer.CustomerList.Params;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.CustomerList;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;
    using Microsoft.Extensions.Configuration;

    public class CustomerListController : BaseController
    {
        private readonly ICustomerListService _customerListService;
        private readonly ICustomerService _customerService;
        private readonly AuthorizationService _authorizationService;

        public CustomerListController(
            ICustomerService customerService,
            ICustomerListService customerListService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _customerListService = customerListService;
            _customerService = customerService;
            _authorizationService = authorizationService;
        }
        private const int DefaultPageSize = 50;

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public IActionResult Index(int? custId, int? roleId)
        {
            var groupName = Request.Query["groupName"].ToString();
            groupName = string.IsNullOrEmpty(groupName) ? CurrentSession.CurrencyGroup : groupName;

            return PartialViewBase(new CustomerListViewModel
            {
                CustId = custId,
                RoleId = roleId,
                HasReadProtectCustomerFunction = _authorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ProtectCustomer),
                IsMultipleCurrencyGroup = Config.IsMultipleCurrencyGroup(groupName)
            });
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult Seach([FromBody] SeachCustomerListRequest request)
        {
            var result = _customerListService.Search(new SearchCustomerListParams
            {
                GroupName = request.GroupName,
                GroupId = request.GroupId,
                IsDataLimited = request.IsShowSuperLimitDate ?? false,
                IsNoDownlines = request.IsNoDownlines ?? false,
                UplineId = request.UplineId ?? 0,
                UplineRoleId = request.UplineRoleId ?? 0,
                Status = request.Status ?? 0,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                CurrencyIds = request.CurrencyIds,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize,
                IsDirectMember = request.IsDirectMember,
                ServiceType = request.ServiceType
            }, _authorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP));
            
            return Ok(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public IActionResult GetCustomerStatus(int custId)
        {
            var products = _customerService.GetCustomerAllStatus(custId, Config.SettingStatus);
            return Ok(products);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public IActionResult GetExportToExcel([FromQuery] SeachCustomerListRequest request)
        {
            var status = request.GetStatusString();
            var roleId = request.UplineRoleId.HasValue ? request.UplineRoleId.Value - 1 : 4;
            var uplineRoleId = request.UplineRoleId ?? 0;
            var customerList = _customerListService.Search(new SearchCustomerListParams
            {
                GroupName = request.GroupName,
                GroupId = request.GroupId,
                IsDataLimited = request.IsShowSuperLimitDate ?? false,
                IsNoDownlines = request.IsNoDownlines ?? false,
                UplineId = request.UplineId ?? 0,
                UplineRoleId = uplineRoleId,
                Status = request.Status ?? 0,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                CurrencyIds = request.CurrencyIds,
                PageIndex = 1,
                PageSize = Int32.MaxValue - 1,
                IsDirectMember = request.IsDirectMember,
                ServiceType = request.ServiceType
            }, _authorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP));

            var builder = new CustomerListPageBuilder(
                customerList.CustomerList,
                false,
                status,
                roleId,
                uplineRoleId,
                request.IsShowSuperLimitDate ?? false,
                request.IsNoDownlines ?? false,
                customerList.HasSubGroups,
                Config.IsMultipleCurrencyGroup(request.GroupName), 
                request.IsDirectMember);
            var excelStream = builder.Build();
            var fileName = request.IsDirectMember ? $"{status}{uplineRoleId.DisplayRoleName()}DirectMemberList{DateTime.Now:yyyyMMdd}.xls" : $"{status}{roleId.DisplayRoleName()}List{DateTime.Now:yyyyMMdd}.xls";

            excelStream.Position = 0;
            return File(excelStream, "application/vnd.ms-excel", fileName);
        }
    }
}
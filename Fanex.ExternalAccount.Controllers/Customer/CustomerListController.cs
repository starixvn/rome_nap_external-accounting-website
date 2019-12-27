namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests.CustomerList;
    using Fanex.ExternalAccount.Core.Authorization;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Excel.Customer.CustomerListPage;
    using Fanex.ExternalAccount.Service.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Customer.CustomerList.Params;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.ExternalAccount.ViewModel.Customer.CustomerList;
    using System;
    using System.Web.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class CustomerListController : CustomerBase
    {
        private readonly ICustomerListService _customerListService;
        private readonly ICustomerService _customerService;
        private readonly AuthorizationService _authorizationService;
        private const int DefaultPageSize = 50;

        public CustomerListController() : this(null, null, null) { }

        public CustomerListController(ICustomerService customerService, ICustomerListService customerListService, AuthorizationService authorizationService)
        {
            _customerService = customerService ?? new CustomerService();
            _customerListService = customerListService ?? new CustomerListService();
            _authorizationService = authorizationService ?? new AuthorizationServiceImpl();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public ActionResult Index(int? custId, int? roleId) => PartialViewBase(new CustomerListViewModel { CustId = custId, RoleId = roleId });

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult Seach(SeachCustomerListRequest request)
        {
            return Json(_customerListService.Search(new SearchCustomerListParams
            {
                GroupName = request.GroupName,
                IsDataLimited = request.IsShowSuperLimitDate ?? false,
                UplineId = request.UplineId ?? 0,
                UplineRoleId = request.UplineRoleId ?? 0,
                Status = request.Status ?? 0,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize
            }, _authorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP)));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult GetCustomerStatus(int custId)
        {
            return Json(_customerService.GetCustomerAllStatus(custId));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticationType.Read)]
        public void GetExportToExcel(SeachCustomerListRequest request)
        {
            var status = request.GetStatusString();
            var roleId = request.UplineRoleId.HasValue ? request.UplineRoleId.Value - 1 : 4;
            var customerList = _customerListService.Search(new SearchCustomerListParams
            {
                GroupName = request.GroupName,
                IsDataLimited = request.IsShowSuperLimitDate ?? false,
                UplineId = request.UplineId ?? 0,
                UplineRoleId = request.UplineRoleId ?? 0,
                Status = request.Status ?? 0,
                UserId = CurrentSession.UserId,
                UserName = request.UserName,
                PageIndex = 1,
                PageSize = Int32.MaxValue - 1
            }, _authorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ViewVNIP));

            var builder = new CustomerListPageBuilder(
                customerList.CustomerList,
                false,
                status,
                roleId,
                request.IsShowSuperLimitDate ?? false);
            var excelStream = builder.Build();
            var fileName = $"{status}{roleId.DisplayRoleName()}List{DateTime.Now.ToString("yyyyMMdd")}.xls";

            ExportToExcel(excelStream, fileName);
        }
    }
}

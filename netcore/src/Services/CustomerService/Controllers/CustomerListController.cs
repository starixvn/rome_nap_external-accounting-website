using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.Core.Context;
using System;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (CustomerList)
    public class CustomerListController : ControllerBase
    {
        private readonly ICustomerListService _customerListService;
        private readonly ICustomerService _customerService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrentSession _currentSession;
        private const int DefaultPageSize = 50;

        public CustomerListController(
            ICustomerListService customerListService,
            ICustomerService customerService,
            IAuthorizationService authorizationService,
            ICurrentSession currentSession)
        {
            _customerListService = customerListService;
            _customerService = customerService;
            _authorizationService = authorizationService;
            _currentSession = currentSession;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index([FromQuery] int? custId, [FromQuery] int? roleId)
        {
            // TODO: Implement proper group name logic
            var groupName = "default"; // HttpContext.Request["groupName"] fallback

            return Ok(new CustomerListViewModel
            {
                CustId = custId,
                RoleId = roleId,
                HasReadProtectCustomerFunction = true, // TODO: _authorizationService.IsAuthorized(_currentSession.UserId, eFunctions.ProtectCustomer)
                IsMultipleCurrencyGroup = false // TODO: Config.IsMultipleCurrencyGroup(groupName)
            });
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult Search([FromBody] SearchCustomerListRequest request)
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
                UserId = _currentSession.UserId,
                UserName = request.UserName,
                CurrencyIds = request.CurrencyIds,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize,
                IsDirectMember = request.IsDirectMember,
                ServiceType = request.ServiceType
            }, true); // TODO: _authorizationService.IsAuthorized(_currentSession.UserId, eFunctions.ViewVNIP)

            return Ok(result);
        }

        [HttpGet]
        [Route("GetCustomerStatus")]
        public IActionResult GetCustomerStatus([FromQuery] int custId)
        {
            // TODO: var products = _customerService.GetCustomerAllStatus(custId, Config.SettingStatus);
            var products = new[] { new { id = 1, name = "Product 1" } }; // Placeholder
            return Ok(products);
        }

        [HttpPost]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel([FromBody] SearchCustomerListRequest request)
        {
            // TODO: Implement Excel export functionality
            // var status = request.GetStatusString();
            // var roleId = request.UplineRoleId.HasValue ? request.UplineRoleId.Value - 1 : 4;
            // var uplineRoleId = request.UplineRoleId ?? 0;
            // var customerList = _customerListService.Search(...);
            // var builder = new CustomerListPageBuilder(...);
            // var excelStream = builder.Build();
            // var fileName = request.IsDirectMember ? $"{status}{uplineRoleId.DisplayRoleName()}DirectMemberList{DateTime.Now:yyyyMMdd}.xls" : $"{status}{roleId.DisplayRoleName()}List{DateTime.Now:yyyyMMdd}.xls";
            // return File(excelStream, "application/vnd.ms-excel", fileName);

            return Ok(new { message = "Excel export not yet implemented", date = DateTime.Now });
        }
    }

    // TODO: Define proper interfaces and services
    public interface ICustomerListService
    {
        object Search(SearchCustomerListParams parameters, bool hasViewVNIPPermission);
    }

    public class CustomerListService : ICustomerListService
    {
        public object Search(SearchCustomerListParams parameters, bool hasViewVNIPPermission)
        {
            // TODO: Implement actual service logic
            return new
            {
                CustomerList = new[] { new { id = 1, name = "Customer 1" } },
                HasSubGroups = false
            };
        }
    }

    // TODO: Define proper request/response models
    public class SearchCustomerListRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public bool? IsShowSuperLimitDate { get; set; }
        public bool? IsNoDownlines { get; set; }
        public int? UplineId { get; set; }
        public int? UplineRoleId { get; set; }
        public int? Status { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CurrencyIds { get; set; } = string.Empty;
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public bool? IsDirectMember { get; set; }
        public int? ServiceType { get; set; }

        public string GetStatusString()
        {
            // TODO: Implement proper status string logic
            return "Active";
        }
    }

    public class CustomerListViewModel
    {
        public int? CustId { get; set; }
        public int? RoleId { get; set; }
        public bool HasReadProtectCustomerFunction { get; set; }
        public bool IsMultipleCurrencyGroup { get; set; }
    }

    // TODO: Define proper parameter models
    public class SearchCustomerListParams
    {
        public string GroupName { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public bool IsDataLimited { get; set; }
        public bool IsNoDownlines { get; set; }
        public int UplineId { get; set; }
        public int UplineRoleId { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CurrencyIds { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool? IsDirectMember { get; set; }
        public int? ServiceType { get; set; }
    }
}
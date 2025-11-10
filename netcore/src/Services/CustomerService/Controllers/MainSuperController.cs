using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (EditCustomer)
    public class MainSuperController : ControllerBase
    {
        private readonly ISuperService _superService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;

        public MainSuperController(ISuperService superService, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            _superService = superService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("EditInfo")]
        public IActionResult EditInfo([FromQuery] int custId)
        {
            var model = _superService.GetSuperInfo(custId);

            model.FirstName = WebUtility.HtmlDecode(model.FirstName);
            model.LastName = WebUtility.HtmlDecode(model.LastName);
            model.Phone = WebUtility.HtmlDecode(model.Phone);
            model.MobilePhone = WebUtility.HtmlDecode(model.MobilePhone);
            model.Email = WebUtility.HtmlDecode(model.Email);
            model.Fax = WebUtility.HtmlDecode(model.Fax);
            model.Remark = WebUtility.HtmlDecode(model.Remark);

            return Ok(model);
        }

        [HttpGet]
        [Route("GetInfo")]
        public IActionResult GetInfo([FromQuery] int superId)
        {
            var model = _superService.GetSuperInfo(superId);
            return Ok(model);
        }

        [HttpPost]
        [Route("EditInfoPost")]
        public IActionResult EditInfoPost([FromBody] CustomerInfo customer)
        {
            var criteria = new UpdateSuperInfoCommand(
                customer.CustId,
                customer.FirstName,
                customer.LastName,
                customer.Phone,
                customer.MobilePhone,
                customer.Fax,
                customer.Remark,
                customer.Email,
                _currentContext.Ip4Address,
                _currentSession.CustId);

            var result = _superService.UpdateSuperInfo(criteria);

            return Ok(new { result.ErrorCode, result.ErrorMessage, result.IsSuccess });
        }
    }

    // TODO: Define proper interfaces and models
    public interface ISuperService
    {
        CustomerInfo GetSuperInfo(int custId);
        (int ErrorCode, string ErrorMessage, bool IsSuccess) UpdateSuperInfo(UpdateSuperInfoCommand criteria);
    }

    public class SuperService : ISuperService
    {
        public CustomerInfo GetSuperInfo(int custId)
        {
            // TODO: Implement actual service logic
            return new CustomerInfo
            {
                CustId = custId,
                FirstName = "John",
                LastName = "Doe",
                Phone = "123-456-7890",
                MobilePhone = "098-765-4321",
                Email = "john.doe@example.com",
                Fax = "123-456-7891",
                Remark = "Test remark"
            };
        }

        public (int ErrorCode, string ErrorMessage, bool IsSuccess) UpdateSuperInfo(UpdateSuperInfoCommand criteria)
        {
            // TODO: Implement actual service logic
            return (0, "Success", true);
        }
    }

    // TODO: Define proper models
    public class CustomerInfo
    {
        public int CustId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }

    public class UpdateSuperInfoCommand
    {
        public UpdateSuperInfoCommand(int custId, string firstName, string lastName, string phone,
                                    string mobilePhone, string fax, string remark, string email,
                                    string ipAddress, int sessionCustId)
        {
            CustId = custId;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            MobilePhone = mobilePhone;
            Fax = fax;
            Remark = remark;
            Email = email;
            IpAddress = ipAddress;
            SessionCustId = sessionCustId;
        }

        public int CustId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Phone { get; }
        public string MobilePhone { get; }
        public string Fax { get; }
        public string Remark { get; }
        public string Email { get; }
        public string IpAddress { get; }
        public int SessionCustId { get; }
    }
}
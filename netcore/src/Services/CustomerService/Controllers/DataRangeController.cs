using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.DataRange
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataRangeController : ControllerBase
    {
        private readonly IDataRangeService _service;
        private readonly ICurrentSession _currentSession;

        public DataRangeController(IDataRangeService service, ICurrentSession currentSession)
        {
            _service = service;
            _currentSession = currentSession;
        }

        [HttpGet]
        [Route("Index")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Index([FromQuery] string custIds)
        {
            var dataRange = string.IsNullOrWhiteSpace(custIds) || custIds.Contains(",")
                ? new CustomerDataRange { Type = CustomerDataRangeType.None }
                : _service.Select(new SelectDataRangeCriteria { CustId = custIds });

            var viewModel = new CustomerDataRangeViewModel
            {
                CustIds = custIds,
                CustomerDataRange = dataRange
            };

            return Ok(viewModel);
        }

        [HttpPost]
        [Route("Update")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Update([FromBody] UpdateDataRangeCriteria criteria)
        {
            criteria.UserId = _currentSession.UserId;

            var result = _service.Update(criteria);

            return Ok(result);
        }
    }

    // TODO: Define proper interfaces and models
    public interface IDataRangeService
    {
        CustomerDataRange Select(SelectDataRangeCriteria criteria);
        object Update(UpdateDataRangeCriteria criteria);
    }

    public class DataRangeService : IDataRangeService
    {
        public CustomerDataRange Select(SelectDataRangeCriteria criteria)
        {
            // TODO: Implement actual service logic
            return new CustomerDataRange { Type = CustomerDataRangeType.None };
        }

        public object Update(UpdateDataRangeCriteria criteria)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    // TODO: Define proper models
    public class CustomerDataRange
    {
        public CustomerDataRangeType Type { get; set; }
    }

    public enum CustomerDataRangeType
    {
        None = 0
    }

    public class SelectDataRangeCriteria
    {
        public string CustId { get; set; } = string.Empty;
    }

    public class UpdateDataRangeCriteria
    {
        public int UserId { get; set; }
        // TODO: Add other properties as needed
    }

    public class CustomerDataRangeViewModel
    {
        public string CustIds { get; set; } = string.Empty;
        public CustomerDataRange CustomerDataRange { get; set; } = new CustomerDataRange();
    }
}

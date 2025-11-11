namespace Fanex.ExternalAccount.Controllers.DataRange
{
    using Core.Base;
    using Core.Shared;
    using DataAccess.Criteria.DataRange;
    using DTO.DataRange;
    using Service.DataRange;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class DataRangeController : BaseController
    {
        private readonly IDataRangeService service;

        public DataRangeController()
        {
            service = new DataRangeService();
        }

        public DataRangeController(IDataRangeService service)
        {
            this.service = service ?? new DataRangeService();
        }

        public IActionResult Index(string custIds)
        {
            var dataRange = string.IsNullOrWhiteSpace(custIds) || custIds.IndexOf(",") != -1
                ? new CustomerDataRange { Type = CustomerDataRangeType.None }
                : service.Select(new SelectDataRangeCriteria { CustId = custIds });
            var viewModel = new CustomerDataRangeViewModel
            {
                CustIds = custIds,
                CustomerDataRange = dataRange
            };

            return PartialViewBase(viewModel);
        }

        [HttpPost]
        public IActionResult Update(UpdateDataRangeCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            var result = service.Update(criteria);
            return Ok(result);
        }
    }
}

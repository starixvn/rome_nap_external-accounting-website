namespace Fanex.ExternalAccount.Controllers.DataRange
{
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using DataAccess.Criteria.DataRange;
    using DTO.DataRange;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Service.DataRange;
    using ViewModels;

    public class DataRangeController : BaseController
    {
        private readonly IDataRangeService service;

        public DataRangeController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService,
            IDataRangeService service)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.service = service;
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

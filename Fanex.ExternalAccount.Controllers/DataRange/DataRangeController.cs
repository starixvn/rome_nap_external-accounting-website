namespace Fanex.ExternalAccount.Controllers.DataRange
{
    using System.Web.Mvc;
    using Core.Base;
    using DataAccess.Criteria.DataRange;
    using Service.DataRange;

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

        public ActionResult Index(string custIds)
        {
            var viewModel = custIds ?? string.Empty;

            return PartialViewBase(viewModel);
        }

        [HttpPost]
        public JsonResult Update(UpdateDataRangeCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            var result = service.Update(criteria);

            return Json(result);
        }
    }
}

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;
    using Fanex.ExternalAccount.Service.Customer.ParlayMaxPayout;
    using System.Web.Mvc;

    public class ParlayMaxPayoutController : BaseController
    {
        private readonly IParlayMaxPayoutService _parlayMaxPayoutService;

        public ParlayMaxPayoutController() : this(null) { }

        public ParlayMaxPayoutController(IParlayMaxPayoutService parlayMaxPayoutService)
        {
            _parlayMaxPayoutService = parlayMaxPayoutService ?? new ParlayMaxPayoutService();
        }

        public ActionResult Index(ParlayMaxPayoutSelectRequest request)
        {
            if (IsMultiple(request))
            {
                request.CustId = GetFirstCustId(request.CustIds);
            }

            var model = _parlayMaxPayoutService.Select(request);
            return PartialViewBase(model);
        }

        [AntiXSRFFilter]
        public JsonResult Update(ParlayMaxPayoutUpdateRequest request)
        {
            return Json(_parlayMaxPayoutService.Update(request));
        }

        [AntiXSRFFilter]
        public JsonResult UpdateMultiple(ParlayMaxPayoutUpdateMultipleRequest request)
        {
            return Json(_parlayMaxPayoutService.UpdateMultiple(request));
        }

        private static bool IsMultiple(ParlayMaxPayoutSelectRequest request)
        {
            return request.CustId == 0 && !string.IsNullOrWhiteSpace(request.CustIds);
        }

        private static int GetFirstCustId(string custIds) => int.Parse(custIds.Split(new[] { ',' }, 2)[0]);
    }
}

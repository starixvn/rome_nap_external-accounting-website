namespace Fanex.ExternalAccount.Controllers.ViewMaxPayoutSettingInformation
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.ViewMaxPayoutSettingInformation;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    public class ViewMaxPayoutSettingInformationController : Controller
    {
        private readonly ViewMaxPayoutSettingInformationService service;

        public ViewMaxPayoutSettingInformationController() : this(null) { }

        public ViewMaxPayoutSettingInformationController(ViewMaxPayoutSettingInformationService service)
        {
            this.service = service ?? new ViewMaxPayoutSettingInformationService();
        }

        public IActionResult Index()
        {
            var betTypeIds = new[] { 1, 3, 5, 7, 8, 15, 20, 21, 153, 501, 609, 610, 612, 701, 704, 705, 9400, 9404, 9405, 9406, 9407, 9408, 9409 };
            var sportTypeIds = new[] { 43, 44 };

            ViewData["BetTypeNames"] = betTypeIds.Select(betTypeId =>
            {
                if (betTypeId == 5)
                {
                    return "1X2";
                }

                return CacheRef.GetBettype(betTypeId.ToString());
            }).ToArray();
            ViewData["SportTypeNames"] = string.Join(" and ", sportTypeIds.Select(sportTypeId => CacheRef.GetSportName(sportTypeId.ToString())));
            ViewData["SabaSports"] = service.GetSabaSports(string.Empty);

            return View("~/Views/MaxPayoutSettingInformation/Index.cshtml");
        }
    }
}
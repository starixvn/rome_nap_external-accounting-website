namespace Fanex.ExternalAccount.Controllers.ViewMaxPayoutSettingInformation
{
    using Fanex.ExternalAccount.Core.Shared;
    using System.Linq;
    using System.Web.Mvc;

    public class ViewMaxPayoutSettingInformationController : Controller
    {
        public ActionResult Index()
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

            return View("~/Views/MaxPayoutSettingInformation/Index.cshtml");
        }
    }
}
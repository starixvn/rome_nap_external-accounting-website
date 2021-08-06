namespace Fanex.ExternalAccount.Controllers.ViewMaxPayoutSettingInformation
{
    using System.Web.Mvc;

    public class ViewMaxPayoutSettingInformationController : Controller
    {
        public ActionResult Index()
            => View("~/Views/MaxPayoutSettingInformation/Index.cshtml");
    }
}
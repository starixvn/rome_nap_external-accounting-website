namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using CustomerSetting.Module.Core.Web.Controllers;

    public class OTPController : BaseController 
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerifyOTP()
        {
            return Redirect("~/Account/ExternalGroup");
        }
    }
}
namespace Fanex.ExternalAccount.Controllers.Customer
{
    using CustomerSetting.Module.Core.Web.Controllers;
    using DTO.OTP;
    using Service.OTP;
    using System.Web.Mvc;
    using CurrentSession = Core.Context.CurrentSession;

    public class OTPController : BaseController 
    {
        private readonly ICustomerOtpService customerOtpService;
        private readonly CurrentSession _currentSession;

        public OTPController()
        {
            _currentSession = new CurrentSession();
            customerOtpService = new CustomerOtpService();
        }

        public ActionResult Index(bool redirect = false)
        {
            ViewBag.redirect = redirect;
            ViewBag.username = _currentSession.UserName;
            return View();
        }

        public ActionResult VerifyOTP(string otp)
             => customerOtpService.VerifyOtp(new CustomerOtp
             {
                 CustId = _currentSession.UserId,
                 Otp = otp
             }) ? VerifyOTPSuccessful() : RedirectToVerifyOTPPage();

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?redirect=true");

        private RedirectResult VerifyOTPSuccessful()
        {
            _currentSession.IsLogin = true;

            return _currentSession.HasMultipleGroupsPermission ? RedirectToChooseGroupPage() : RedirectToHomePage();
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectResult RedirectToChooseGroupPage() => Redirect("~/Account/ExternalGroup");
    }
}
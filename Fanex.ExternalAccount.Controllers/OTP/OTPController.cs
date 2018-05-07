namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Authentication;
    using Core.Context;
    using CustomerSetting.Module.Core.Web.Controllers;
    using CustomerSetting.Security.Authenticate;
    using DTO.OTP;
    using Service.OTP;
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

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);
            ViewBag.code = Encryptor.RandomCaptcha;

            return View();
        }

        private static string DecryptOTP(string encryptedOTP, string key) =>
          Encryptor.Decrypt(encryptedOTP, key);

        [HttpPost]
        public ActionResult VerifyOTP(string otp, string _pt, string _cd, string __RequestVerificationToken)
        {
            if(!AntiXSRFUtil.ValidateAntiXSRF(Request, __RequestVerificationToken))
            {
                return RedirectToVerifyOTPPage();
            }

            var rawOTP = DecryptOTP(_pt, _cd);
            if(!string.IsNullOrEmpty(rawOTP) || rawOTP.Contains(_currentSession.UserName))
            {
                return VerifyOTPSuccessful(_cd);
            }

            return customerOtpService.VerifyOtp(new CustomerOtp
            {
                CustId = _currentSession.UserId,
                Otp = otp
            }) ? VerifyOTPSuccessful(_cd) : RedirectToVerifyOTPPage();
        }

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?redirect=true");

        private RedirectResult VerifyOTPSuccessful(string code)
        {
            _currentSession.IsLogin = true;

            return _currentSession.HasMultipleGroupsPermission ? RedirectToChooseGroupPage(code) : RedirectToHomePage();
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectResult RedirectToChooseGroupPage(string code) => Redirect("~/Account/ExternalGroup?code="+ code);
    }
}
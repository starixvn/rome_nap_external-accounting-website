namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Authentication;
    using Core.Context;
    using DTO.OTP;
    using Service.OTP;
    using CurrentSession = Core.Context.CurrentSession;

    public class OTPController : Controller
    {
        private const string String_Empty = "";

        private readonly ICustomerOtpService customerOtpService;
        private readonly CurrentSession _currentSession;

        public OTPController()
        {
            _currentSession = new CurrentSession();
            customerOtpService = new CustomerOtpService();
        }

        public ActionResult Index(bool redirect = false)
        {
            var otpModel = customerOtpService.EnableOTP(_currentSession.CustId);
            if (!otpModel.Enabled)
            {
                return VerifyOTPSuccessful();
            }

            _currentSession.OTPSecretKey = otpModel.SecretKey;

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);

            return View(new CustomerOtpViewModel {
                Redirect = redirect,
                Username = _currentSession.UserName,
                Code = Encryptor.RandomCaptcha
            });
        }

        [HttpPost]
        public ActionResult VerifyOTP(string otp, string _pt, string _cd, string __RequestVerificationToken)
        {
            if(!AntiXSRFUtil.ValidateAntiXSRF(Request, __RequestVerificationToken))
            {
                return RedirectToVerifyOTPPage();
            }

            var rawOTP = Encryptor.Decrypt(_pt, _cd);
            if(!string.IsNullOrEmpty(rawOTP) || rawOTP.Contains(_currentSession.UserName))
            {
                return VerifyOTPSuccessful(_cd);
            }

            return customerOtpService.VerifyOtp(otp, _currentSession.OTPSecretKey) ? VerifyOTPSuccessful(_cd) : RedirectToVerifyOTPPage();
        }

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?redirect=true");

        private RedirectResult VerifyOTPSuccessful(string code = String_Empty)
        {
            _currentSession.IsLogin = true;

            return _currentSession.HasMultipleGroupsPermission ? RedirectToChooseGroupPage(code) : RedirectToHomePage(code);
        }

        private RedirectResult RedirectToHomePage(string code) => Redirect("~/Home/Index?_cd=" + code);

        private RedirectResult RedirectToChooseGroupPage(string code) => Redirect("~/Account/ExternalGroup?_cd=" + code);
    }
}
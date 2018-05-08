namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Authentication;
    using Core.Context;
    using Service.OTP;
    using ViewModels;
    using CurrentSession = Core.Context.CurrentSession;

    public class OTPController : Controller
    {
        private const string String_Empty = "";

        private readonly ICustomerOtpService customerOtpService;
        private readonly CurrentSession currentSession;

        public OTPController()
        {
            currentSession = new CurrentSession();
            customerOtpService = new CustomerOtpService();
        }

        public ActionResult Index(bool reverify = false)
        {
            var customerOTP = customerOtpService.EnableOTP(currentSession.CustId);
            SetUserOTPSession(customerOTP.Enabled, customerOTP.SecretKey);

            if (!currentSession.EnaledOTP)
            {
                return VerifyOTPSuccessful();
            }

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);

            return View(new CustomerOtpViewModel {
                Reverify = reverify,
                Username = currentSession.UserName,
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
            if(!string.IsNullOrEmpty(rawOTP) && rawOTP.Equals($"{currentSession.UserName}_pt"))
            {
                return VerifyOTPSuccessful(_cd);
            }

            return customerOtpService.VerifyOtp(otp, currentSession.OTPSecretKey) ? VerifyOTPSuccessful(_cd) : RedirectToVerifyOTPPage();
        }

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?reverify=true");

        private RedirectResult VerifyOTPSuccessful(string code = String_Empty)
        {
            currentSession.IsLogin = true;

            return currentSession.HasMultipleGroupsPermission ? RedirectToChooseGroupPage(code) : RedirectToHomePage(code);
        }

        private RedirectResult RedirectToHomePage(string code) => Redirect("~/Home/Index?_cd=" + code);

        private RedirectResult RedirectToChooseGroupPage(string code) => Redirect("~/Account/ExternalGroup?_cd=" + code);

        private void SetUserOTPSession(bool enabled, string secretKey)
        {
            currentSession.EnaledOTP = enabled;
            currentSession.OTPSecretKey = secretKey;
        }
    }
}
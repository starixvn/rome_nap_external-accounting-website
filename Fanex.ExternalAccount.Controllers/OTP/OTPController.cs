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
        private readonly ICustomerOtpService customerOtpService;
        private readonly CurrentSession currentSession;

        private const int LimitedFailure = 5;

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

            return View(new CustomerOtpViewModel {
                Reverify = reverify,
                Username = currentSession.UserName,
                Code = currentSession.OTPEncryptCode,
                Reset = customerOTP.Reset
            });
        }

        [HttpPost]
        public ActionResult VerifyOTP(VerifyOtpCommand command)
        {
            if(!AntiXSRFUtil.ValidateAntiXSRF(Request, command.__RequestVerificationToken))
            {
                return RedirectToVerifyOTPPage();
            }

            var rawOTP = Encryptor.Decrypt(command._pt, command._cd);
            if(!string.IsNullOrEmpty(rawOTP) && rawOTP.Equals(GetOTPQuote()))
            {
                return VerifyOTPSuccessful();
            }

            var verifyOTPSusscess = customerOtpService.VerifyOtp(command._otp, currentSession.OTPSecretKey);

            return verifyOTPSusscess ? VerifyOTPSuccessful(command._reset) : VerifyOTPFailed();
        }

        private RedirectResult VerifyOTPFailed()
        {
            currentSession.OTPFailedCounter += 1;

            return currentSession.OTPFailedCounter.Equals(LimitedFailure) ? RedirectToLoginPage() : RedirectToVerifyOTPPage();
        }

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?reverify=true");

        private RedirectResult VerifyOTPSuccessful(bool reset = false)
        {
            if(reset && !customerOtpService.TurnResetOTPOff(currentSession.CustId).IsSuccess)
            {
                return RedirectToVerifyOTPPage();
            }

            currentSession.IsLogin = true;

            return currentSession.HasMultipleGroupsPermission ? RedirectToChooseGroupPage() : RedirectToHomePage();
        }

        private RedirectResult RedirectToLoginPage()
        {
            currentSession.OTPFailedCounter = 0;
            currentSession.InvalidOTP = true;

            return Redirect("~/Account/Login");
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index");

        private RedirectResult RedirectToChooseGroupPage() => Redirect("~/Account/ExternalGroup");

        private string GetOTPQuote() => $"{currentSession.UserName} is already verified OTP.";

        private void SetUserOTPSession(bool enabled, string secretKey)
        {
            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);
            currentSession.OTPEncryptCode = Encryptor.RandomCaptcha;
            currentSession.EnaledOTP = enabled;
            currentSession.OTPSecretKey = secretKey;
        }
    }
}
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
            SetUserOTPSession(reverify);

            if (currentSession.OTPFailedCounter.Equals(LimitedFailure))
            {
                return RedirectToLoginPage();
            }

            if (!currentSession.EnaledOTP)
            {
                return VerifyOTPSuccessful();
            }

            return View(new CustomerOtpViewModel {
                Reverify = reverify,
                Username = currentSession.UserName,
                Code = currentSession.OTPEncryptCode,
                ModifyDate = currentSession.OTPModifyDate
            });
        }

        [HttpPost]
        public ActionResult VerifyOTP(VerifyOtpCommand command)
        {
            if (!AntiXSRFUtil.ValidateAntiXSRF(Request, command.__RequestVerificationToken))
            {
                return RedirectToVerifyOTPPage();
            }

            var rawOTPQuote = Encryptor.Decrypt(command._pt, command._cd);

            if(command._md.Equals(currentSession.OTPModifyDate)
                && !string.IsNullOrEmpty(rawOTPQuote) 
                && rawOTPQuote.Equals(GetOTPQuote()))
            {
                return VerifyOTPSuccessful();
            }

            var verifyOTPSusscess = customerOtpService.VerifyOtp(command._otp, currentSession.OTPSecretKey);

            return verifyOTPSusscess ? VerifyOTPSuccessful() : RedirectToVerifyOTPPage();
        }

        private RedirectResult RedirectToVerifyOTPPage() => Redirect("~/OTP/Index?reverify=true");

        private RedirectResult VerifyOTPSuccessful()
        {
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

        private void SetUserOTPSession(bool reverify)
        {
            var customerOtp = customerOtpService.EnableOTP(currentSession.CustId);

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);
            currentSession.OTPEncryptCode = Encryptor.RandomCaptcha;
            currentSession.EnaledOTP = customerOtp.Enabled;
            currentSession.OTPSecretKey = customerOtp.SecretKey;
            currentSession.OTPModifyDate = customerOtp.ModifyDate;

            if (reverify)
            {
                currentSession.OTPFailedCounter += 1;
            }
        }
    }
}
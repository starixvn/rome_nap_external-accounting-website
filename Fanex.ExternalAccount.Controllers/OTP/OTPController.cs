namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using Authentication;
    using Core.Context;
    using Logging.Extensions.Web;
    using Resources;
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

        public ActionResult Index()
        {
            SetUserOTPSession();

            if (!currentSession.EnabledOTP)
            {
                return LoginSuccessful();
            }

            return View(new CustomerOtpViewModel {
                Username = currentSession.UserName,
                Code = currentSession.OTPEncryptCode,
                ModifiedDate = currentSession.OTPModifiedDate,
                FailedCounter = currentSession.OTPFailedCounter,
                RequiredInputOTP = currentSession.InvalidOTP
            });
        }

        [HttpPost]
        public ActionResult VerifyOTP(VerifyOtpCommand command)
        {
            if (!AntiXSRFUtil.ValidateAntiXSRF(Request, command.__RequestVerificationToken))
            {
                return LoginFail();
            }

            var rawOTPQuote = Encryptor.Decrypt(command._pt, command._cd);

            if(command._md.Equals(currentSession.OTPModifiedDate)
                && !string.IsNullOrEmpty(rawOTPQuote) 
                && rawOTPQuote.Equals(GetOTPQuote()))
            {
                return LoginSuccessful();
            }

            var verifyOTPSuccess = customerOtpService.VerifyOtp(command._otp, currentSession.OTPSecretKey);

            return verifyOTPSuccess ? LoginSuccessful() : LoginFail();
        }

        private RedirectResult LoginFail()
        {
            currentSession.OTPFailedCounter++;

            if (currentSession.OTPFailedCounter.Equals(LimitedFailure))
            {
                ExceptionHandler.Handle(new Exception($"{currentSession.UserName} is failed OTP 5 times."));
                return RedirectToLoginPage();
            }

            return RedirectToOTPPage();
        }

        private RedirectResult RedirectToOTPPage() => Redirect("~/OTP/Index");

        private RedirectResult LoginSuccessful()
        {
            currentSession.IsLogin = true;
            currentSession.InvalidOTP = false;

            return currentSession.IsInternalAdmin ? RedirectToChooseGroupPage() : RedirectToHomePage();
        }

        private RedirectResult RedirectToLoginPage()
        {
            currentSession.OTPFailedCounter = 0;
            currentSession.InvalidOTP = true;
            currentSession.UserName = string.Empty;
            currentSession.UserId = 0;
            currentSession.CustId = 0;
            FormsAuthentication.SignOut();

            return Redirect("~/Account/Login");
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index");

        private RedirectResult RedirectToChooseGroupPage() => Redirect("~/Account/ExternalGroup");

        private string GetOTPQuote() 
            => String.Format(OTP.OTPQuote, currentSession.UserName, currentSession.OTPModifiedDate);

        private void SetUserOTPSession()
        {
            var customerOtp = customerOtpService.GetCustomerOtpDto(currentSession.CustId);

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);
            currentSession.OTPEncryptCode = Encryptor.RandomCaptcha;
            currentSession.EnabledOTP = customerOtp.Enabled;
            currentSession.OTPSecretKey = customerOtp.SecretKey;
            currentSession.OTPModifiedDate = customerOtp.ModifiedDate;
        }
    }
}
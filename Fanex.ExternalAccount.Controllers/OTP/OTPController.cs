namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Module.Core.Web.Controllers;
    using DTO.OTP;
    using Service.OTP;
    using CurrentContext = Core.Context.CurrentContext;
    using CurrentSession = Core.Context.CurrentSession;

    public class OTPController : BaseController 
    {
        private readonly ICustomerOtpService customerOtpService;
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;

        public OTPController()
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
            customerOtpService = new CustomerOtpService();
        }

        public ActionResult Index(bool redirect = false)
        {
            ViewBag.redirect = redirect;
            return View();
        }

        public ActionResult VerifyOTP(string otp)
        {
            if(string.IsNullOrEmpty(otp))
            {
                return Redirect("~/OTP/Index?redirect=true");
            }

            var result = customerOtpService.VerifyOtp(new CustomerOtp
            {
                CustId = 615976,
                SubAccId = 0,
                Otp = otp,
                Language = "E"
            });

           return result.IsSuccess ? Redirect("~/Account/ExternalGroup") : Redirect("~/OTP/Index?redirect=true");
            //return Redirect("~/OTP/Index?redirect=true");
            //return Redirect("~/Account/ExternalGroup");
        }
    }
}
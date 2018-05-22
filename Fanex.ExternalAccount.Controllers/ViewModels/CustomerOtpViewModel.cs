namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    public class CustomerOtpViewModel
    {
        public string Username { get; set; }

        public string Code { get; set; }

        public string ModifiedDate { get; set; }

        public int FailedCounter { get; set; }

        public bool RequiredInputOTP { get; set; }
    }
}
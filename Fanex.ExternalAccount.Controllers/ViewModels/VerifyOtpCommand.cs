namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    public class VerifyOtpCommand
    {
        public string _otp { get; set; }

        public string _pt { get; set; }

        public string _cd { get; set; }

        public string __RequestVerificationToken { get; set; }

        public string _md { get; set; }
    }
}
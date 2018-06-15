namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    public class VerifyOtpCommand
    {
        public string _otp { get; set; }

        public string _pt { get; set; }

        public string _cd { get; set; }

        public string __RequestVerificationToken { get; set; }

        public string _md { get; set; }

        public bool InValid
            => string.IsNullOrEmpty(_otp)
            || string.IsNullOrEmpty(_pt)
            || string.IsNullOrEmpty(_cd)
            || string.IsNullOrEmpty(_md);
    }
}
using System.ComponentModel.DataAnnotations;

namespace Fanex.ExternalAccount.ViewModel.Account
{
    public class LoginModel
    {
        public const int ThresholdFailToShowingCaptcha = 3;

        public LoginModel() { }

        public LoginModel(string loginFailedMessage)
        {
            LoginFailedMessage = loginFailedMessage;
        }



        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Captcha { get; set; }

        public string Code { get; set; }

        public string BrowserSize { get; set; }

        public string __RequestVerificationToken { get; set; }

        public bool IsVisibleCaptcha => (LoginAttempt >= ThresholdFailToShowingCaptcha) || InvalidOTP;

        public int LoginAttempt { get; set; }

        public string LoginFailedMessage { get; set; }

        public bool InvalidOTP { get; set; }

        public string Line { get; set; }

        public string FromLine { get; set; }

        public double TotalSeconds { get; set; }
    }
}
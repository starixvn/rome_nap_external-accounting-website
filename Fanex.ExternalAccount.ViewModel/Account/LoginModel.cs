namespace Fanex.ExternalAccount.ViewModel.Account
{
    using System.ComponentModel.DataAnnotations;

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

        public string __RequestVerificationToken { get; set; }

        public bool IsVisibleCaptcha => LoginAttempt >= ThresholdFailToShowingCaptcha;

        public int LoginAttempt { get; set; }

        public string LoginFailedMessage { get; set; }
    }
}
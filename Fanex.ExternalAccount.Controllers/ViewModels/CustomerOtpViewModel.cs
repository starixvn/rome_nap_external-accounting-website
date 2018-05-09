namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    public class CustomerOtpViewModel
    {
        public string Username { get; set; }

        public bool Reverify { get; set; }

        public string Code { get; set; }

        public bool Reset { get; set; }
    }
}
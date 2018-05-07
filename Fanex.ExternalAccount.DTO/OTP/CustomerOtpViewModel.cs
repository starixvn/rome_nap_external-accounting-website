namespace Fanex.ExternalAccount.DTO.OTP
{
    public class CustomerOtpViewModel
    {
        public string Username { get; set; }

        public bool Redirect { get; set; }

        public string Code { get; set; }
    }
}
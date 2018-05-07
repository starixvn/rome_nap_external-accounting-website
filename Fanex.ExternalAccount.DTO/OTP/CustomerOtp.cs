namespace Fanex.ExternalAccount.DTO.OTP
{
    public class CustomerOtp
    {
        public int CustId { get; set; }

        public string SecretKey { get; set; }

        public bool Enabled { get; set; }
    }
}
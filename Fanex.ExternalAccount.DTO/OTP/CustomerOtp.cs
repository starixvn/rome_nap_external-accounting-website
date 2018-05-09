namespace Fanex.ExternalAccount.DTO.OTP
{
    public class CustomerOtp
    {
        public string SecretKey { get; set; }

        public bool Enabled { get; set; }

        public bool Reset { get; set; }
}
}
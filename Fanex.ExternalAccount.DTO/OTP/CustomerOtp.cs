namespace Fanex.ExternalAccount.DTO.OTP
{
    using System;

    public class CustomerOtp
    {
        public string SecretKey { get; set; }

        public bool Enabled { get; set; }

        public string ModifyDate { get; set; }
    }
}
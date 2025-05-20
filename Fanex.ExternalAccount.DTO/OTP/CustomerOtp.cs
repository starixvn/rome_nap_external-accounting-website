using System;

namespace Fanex.ExternalAccount.DTO.OTP
{
    public class CustomerOtp
    {
        public string SecretKey { get; set; }

        public bool Enabled { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedDateToString => ModifiedDate.Ticks.ToString();
    }
}
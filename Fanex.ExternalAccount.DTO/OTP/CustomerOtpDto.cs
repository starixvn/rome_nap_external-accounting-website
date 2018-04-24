namespace Fanex.ExternalAccount.DTO.OTP
{
    using System;

    public class CustomerOtpDto
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string SecretKey { get; set; }

        public bool Enabled { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool IsDelete { get; set; }
    }
}
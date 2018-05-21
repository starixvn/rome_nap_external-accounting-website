namespace Fanex.ExternalAccount.Service.OTP
{
    using System;
    using DataAccess.OTP;
    using DTO.OTP;
    using ONEbook.OTP.Services;

    public interface ICustomerOtpService
    {
        CustomerOtp GetCustomerOtpDto(int custId);

        bool VerifyOtp(string otp, string secretKey);
    }

    public class CustomerOtpService : ICustomerOtpService
    {
        private readonly OtpService otpService;
        private readonly ICustomerOtpDataAccess customerOtpDataAccess;

        public CustomerOtpService(OtpService otpService = null, ICustomerOtpDataAccess customerOtpDataAccess = null)
        {
            this.otpService = otpService ?? new OtpServiceImpl();
            this.customerOtpDataAccess = customerOtpDataAccess ?? new CustomerOtpDataAccess();
        }

        public CustomerOtp GetCustomerOtpDto(int custId)
        {
            if (custId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(custId), "Cust Id cannot be negative or zero");
            }

           return customerOtpDataAccess.GetCustomerOtpDto(custId);
        }
          
        public bool VerifyOtp(string otp, string secretKey)
        {
            if (string.IsNullOrEmpty(otp))
            {
                return false;
            }

            return otpService.VerifyOtp(secretKey, otp);
        }
    }
}
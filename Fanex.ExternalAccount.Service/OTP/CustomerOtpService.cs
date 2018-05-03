namespace Fanex.ExternalAccount.Service.OTP
{
    using DTO;
    using DTO.OTP;
    using Fanex.ExternalAccount.DataAccess.OTP;
    using ONEbook.OTP.Services;
    using System;

    public class CustomerOtpService : ICustomerOtpService
    {
        private const int SuccessCode = -1;
        private const int ErrorCode = 101;

        private readonly OtpService otpService;
        private readonly ICustomerOtpDataAccess customerOtpDataAccess;

        public CustomerOtpService(OtpService otpService = null, ICustomerOtpDataAccess customerOtpDataAccess = null)
        {
            this.otpService = otpService ?? new OtpServiceImpl();
            this.customerOtpDataAccess = customerOtpDataAccess ?? new CustomerOtpDataAccess();
        }

        public Result VerifyOtp(CustomerOtp param)
        {
            if (param == null || string.IsNullOrEmpty(param.Otp))
            {
                return new Result(100, "Verify fail");
            }

            var customerOtp = GetCustomerOtp(param.CustId);

            var isCorrect = otpService.VerifyOtp(customerOtp.SecretKey, param.Otp);

            return new Result(isCorrect ? SuccessCode : ErrorCode, isCorrect ? "Verify okie" : "Verify fail");
        }

        private CustomerOtp GetCustomerOtp(int custId)
        {
            try
            {
                if (custId <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(custId), "Cust Id cannot be negative or zero");
                }

                return customerOtpDataAccess.GetCustomerOtpDto(custId);
            }
            catch (Exception ex)
            {
                return new CustomerOtp();
            }
        }
    }
}
namespace Fanex.ExternalAccount.Service.OTP
{
    using DTO.OTP;
    using Fanex.ExternalAccount.DataAccess.OTP;
    using ONEbook.OTP.Services;

    public class CustomerOtpService : ICustomerOtpService
    {
        private readonly OtpService otpService;
        private readonly ICustomerOtpDataAccess customerOtpDataAccess;

        public CustomerOtpService(OtpService otpService = null, ICustomerOtpDataAccess customerOtpDataAccess = null)
        {
            this.otpService = otpService ?? new OtpServiceImpl();
            this.customerOtpDataAccess = customerOtpDataAccess ?? new CustomerOtpDataAccess();
        }

        public bool VerifyOtp(CustomerOtp param)
        {
            if (param == null || string.IsNullOrEmpty(param.Otp) || param.CustId <= 0)
            {
                return false;
            }

            var customerOtp = customerOtpDataAccess.GetCustomerOtpDto(param.CustId); 

            return otpService.VerifyOtp(customerOtp.SecretKey, param.Otp);
        }
    }
}
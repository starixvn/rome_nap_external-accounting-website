namespace Fanex.ExternalAccount.Service.OTP
{
    using DTO;
    using DTO.OTP;

    public interface ICustomerOtpService
    {
        bool VerifyOtp(CustomerOtp param);
    }
}
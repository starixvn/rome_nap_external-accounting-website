namespace Fanex.ExternalAccount.Service.OTP
{
    using DTO;
    using DTO.OTP;

    public interface ICustomerOtpService
    {
        Result VerifyOtp(CustomerOtp param);
    }
}
namespace Fanex.ExternalAccount.Service.OTP
{
    using DTO.OTP;

    public interface ICustomerOtpService
    {
        CustomerOtp EnableOTP(int custId);

        bool VerifyOtp(string otp, string secretKey);
    }
}
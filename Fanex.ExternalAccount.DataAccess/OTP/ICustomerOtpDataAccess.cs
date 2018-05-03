namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using DTO.OTP;

    public interface ICustomerOtpDataAccess
    {
        CustomerOtp GetCustomerOtpDto(int custId);
    }
}
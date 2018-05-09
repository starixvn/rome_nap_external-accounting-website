namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using System.Collections;
    using Data;
    using DTO.OTP;
    using Utilities;

    public interface ICustomerOtpDataAccess
    {
        CustomerOtp GetCustomerOtpDto(int custId);
    }

    public class CustomerOtpDataAccess : ICustomerOtpDataAccess
    {
        public CustomerOtp GetCustomerOtpDto(int custId)
        {
            // Cannot use dynamic mapping here due to old fanex.data
            var enabled = false;
            var reset = false;
            var secretKey = string.Empty;

            using (var db = new ObjectDb("EAS_OTP_CustomerOTPSel"))
            {
                var reader = db.ExecuteReader(new Hashtable
                    {
                        { "custid", custId }
                    });

                while (reader.Read())
                {
                    enabled = reader.ToBoolean("Enabled");
                    reset = reader.ToBoolean("Reset");
                    secretKey = reader.ToString("SecretKey"); 
                }

                return new CustomerOtp
                {
                    Enabled = enabled,
                    SecretKey = secretKey,
                    Reset = reset
                };
            }
        }
    }
}
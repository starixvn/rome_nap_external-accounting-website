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
            var Enabled = false;
            var SecretKey = string.Empty;

            using (var db = new ObjectDb("EAS_OTP_CustomerOTPSel"))
            {
                var reader = db.ExecuteReader(new Hashtable
                    {
                        { "custid", custId }
                    });

                while (reader.Read())
                {
                    Enabled = reader.ToBoolean("Enabled");
                    SecretKey = reader.ToString("SecretKey");
                }

                return new CustomerOtp
                {
                    Enabled = Enabled,
                    SecretKey = SecretKey
                };
            }
        }
    }
}
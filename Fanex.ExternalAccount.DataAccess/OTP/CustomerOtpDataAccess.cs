namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using System.Collections;
    using Common;
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
            var secretKey = string.Empty;
            var modifiedDate = string.Empty;

            using (var db = new ObjectDb("EAS_OTP_CustomerOTPSel"))
            {
                var reader = db.ExecuteReader(new Hashtable
                    {
                        { "custid", custId }
                    });

                while (reader.Read())
                {
                    enabled = reader.ToBoolean("Enabled");
                    secretKey = reader.ToString("SecretKey"); 
                    modifiedDate = Converter.ToDateTime(reader.ToString("ModifiedDate")).Ticks.ToString();
                }

                return new CustomerOtp
                {
                    Enabled = enabled,
                    SecretKey = secretKey,
                    ModifiedDate = modifiedDate
                };
            }
        }
    }
}
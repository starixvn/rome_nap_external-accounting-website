namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using System.Collections;
    using System.Data;
    using Common;
    using Data;
    using DTO;
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
            var modifyDate = string.Empty;

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
                    modifyDate = Converter.ToDateTime(reader.ToString("modifyDate")).Ticks.ToString();
                }

                return new CustomerOtp
                {
                    Enabled = enabled,
                    SecretKey = secretKey,
                    ModifyDate = modifyDate
                };
            }
        }
    }
}
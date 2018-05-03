namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using Data;
    using DTO.OTP;
    using System;
    using System.Collections;
    using Utilities;

    public class CustomerOtpDataAccess : ICustomerOtpDataAccess
    {
        public CustomerOtp GetCustomerOtpDto(int custId)
        {
            try
            {
                // Cannot use dynamic mapping here due to old fanex.data
                var CustId = 0;
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
                        CustId = reader.ToInt32("CustID");
                        Enabled = reader.ToBoolean("Enabled");
                        SecretKey = reader.ToString("SecretKey");
                    }

                    return new CustomerOtp
                    {
                        CustId = CustId,
                        Enabled = Enabled,
                        SecretKey = SecretKey
                    };
                }
            }
            catch (Exception ex)
            {
                return new CustomerOtp();
            }
 
        }
    }
}
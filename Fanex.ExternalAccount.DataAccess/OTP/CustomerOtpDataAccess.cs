namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using System.Collections;
    using System.Data;
    using Data;
    using DTO;
    using DTO.OTP;
    using Utilities;

    public interface ICustomerOtpDataAccess
    {
        CustomerOtp GetCustomerOtpDto(int custId);

        Result TurnResetOTPOff(int custId);
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

        public Result TurnResetOTPOff(int custId)
        {
            var result = new Result();

            using (var db = new ObjectDb("EAS_OTP_CustomerOTPReset"))
            {
                db.AddInParameter("CustId", DbType.Int32, custId);
                db.AddOutParameter("ErrCode", DbType.Int32, sizeof(int));
                db.AddOutParameter("ErrMsg", DbType.String, 200);
                db.ExecuteNonQuery();

                result.ErrorCode = db.GetParameterValue("ErrCode").ToString();
                result.ErrorMessage = db.GetParameterValue("ErrMsg").ToString();
            }

            return result;
        }
    }
}
namespace Fanex.ExternalAccount.Service.OTP
{
    using Fanex.NAP.Framework.Data;

    public class GetCustomerOtpCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public override string GetSettingKey()
        {
            return "Age_OTP_CustomerOTPSel";
        }

        public override bool IsValid()
        {
            return CustId > 0
               && SubAccId >= 0;
        }
    }
}
namespace Fanex.ExternalAccount.DataAccess.Criteria.OTP
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Collections;

    public class GetCustomerOtpDtoCriteria : CriteriaBase
    {
        public int CustId { get; }

        public GetCustomerOtpDtoCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey() => "EAS_OTP_CustomerOTPSel";

        public override bool IsValid() => CustId > 0;
    }
}
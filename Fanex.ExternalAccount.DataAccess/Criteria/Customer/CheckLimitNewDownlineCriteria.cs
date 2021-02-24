using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    public class CheckLimitNewDownlineCriteria : CriteriaBase
    {
        public CheckLimitNewDownlineCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "Acc_ProblemAccountLimitDownlineCheck";

        public override bool IsValid()
            => CustId > 0;
    }
}

namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMemberUsedCreditItemsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "uplineid")]
        public int CustId { get; }

        public int ServiceType { get; }

        public GetMemberUsedCreditItemsCriteria(int custId, int serviceType)
        {
            CustId = custId;
            ServiceType = serviceType;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelUsedCredit_Ag";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}














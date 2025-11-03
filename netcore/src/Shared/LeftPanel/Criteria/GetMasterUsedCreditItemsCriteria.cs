namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMasterUsedCreditItemsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "superid")]
        public int CustId { get; }

        public GetMasterUsedCreditItemsCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelUsedCredit_SAg";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
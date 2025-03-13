namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAgentUsedCreditItemsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "masterid")]
        public int CustId { get; }

        public GetAgentUsedCreditItemsCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelUsedCredit_MAg";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
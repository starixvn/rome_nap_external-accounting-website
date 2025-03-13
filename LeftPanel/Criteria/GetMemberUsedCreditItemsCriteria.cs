namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMemberUsedCreditItemsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "agentid")]
        public int CustId { get; }

        public GetMemberUsedCreditItemsCriteria(int custId)
        {
            CustId = custId;
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














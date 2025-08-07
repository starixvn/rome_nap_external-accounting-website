namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAgentGivenCreditCommand : INonQueryCommand
    {
        [SpParam(SqlParamName = "custid")]
        public int Id { get; }

        [SpParam(SqlParamName = "userid")]
        public int UplineId { get; }

        [SpParam(SqlParamName = "newcredit")]
        public decimal NewGivenCredit { get; }

        [SpParam(SqlParamName = "accumcreditallcashmember")]
        public decimal AccumCreditAllCashMember { get; }

        public string Ip { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public UpdateAgentGivenCreditCommand(int id, int uplineId, decimal newGivenCredit, decimal accumCreditAllCashMember, string ip)
        {
            Id = id;
            UplineId = uplineId;
            NewGivenCredit = newGivenCredit;
            AccumCreditAllCashMember = accumCreditAllCashMember;
            Ip = ip;
        }

        public virtual string GetSettingKey()
        {
            return "Acc_CustomerUpdCredit";
        }

        public virtual bool IsValid()
        {
            return Id > 0 && UplineId > 0;
        }
    }
}
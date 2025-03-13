namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class ResetMemberCreditCommand : INonQueryCommand
    {
        public int CustId { get; }
        public int AgentId { get; }
        public bool ClosedOnly { get; }
        public string Ip { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public ResetMemberCreditCommand(int custId, int agentId, bool closedOnly, string ip)
        {
            CustId = custId;
            AgentId = agentId;
            ClosedOnly = closedOnly;
            Ip = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdResetMemberCredit";
        }

        public bool IsValid()
        {
            return CustId > 0 && AgentId > 0;
        }
    }
}
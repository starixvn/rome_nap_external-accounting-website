namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class CloseCustomerCommand : INonQueryCommand
    {
        public int CustId { get; }
        public bool Closed { get; }
        public bool ClearCredit { get; }
        public string CustName { get; }
        public int UserId { get; }
        public string Language { get; }
        public string Ip { get; }
        public int SystemId { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public CloseCustomerCommand(int custId, bool closed, bool clearCredit, string custName, int userId, string language, string ip, int systemId)
        {
            CustId = custId;
            Closed = closed;
            ClearCredit = clearCredit;
            CustName = custName;
            UserId = userId;
            Language = language;
            Ip = ip;
            SystemId = systemId;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdClosed";
        }

        public bool IsValid()
        {
            return CustId > 0;
        }
    }
}

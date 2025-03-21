namespace Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public  class AssignPersonalMessageCommand : INonQueryCommand  
    {
        public AssignPersonalMessageCommand(long messageId, int custId, string custName)
        {
            MessageId = messageId;
            CustId = custId;
            CustName = custName;
            BetTransId = "0";
        }

        public long MessageId { get; }

        [SpParam(SqlParamName = "TargetId")]
        public int CustId { get; }

        [SpParam(SqlParamName = "TargetName")]
        public string CustName { get; }

        public string BetTransId { get; }

        public string GetSettingKey() => "Age_Msg_SetUsrMessage";

        public bool IsValid() => MessageId > 0;
    }
}

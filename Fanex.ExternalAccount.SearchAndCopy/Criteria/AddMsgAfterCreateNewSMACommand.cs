namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.Data.Repository;

    public class AddMsgAfterCreateNewSMACommand : INonQueryCommand
    {
        public AddMsgAfterCreateNewSMACommand(int custId, int subAccId, int roleid)
        {
            CustId = custId;
            SubAccId = subAccId;
            Roleid = roleid;
        }

        public int CustId { get; }

        public int SubAccId { get; }

        public int Roleid { get; }

        public string GetSettingKey() => "Age_MsgCustReadStatus_Ins";

        public bool IsValid() => CustId > 0;
    }
}

namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class UnlockAccessCommand : INonQueryCommand
    {
        public UnlockAccessCommand(int custId, int userId, byte locked, string ip, byte systemId)
        {
            CustId = custId;
            UserId = userId;
            Locked = locked;
            IP = ip;
            SystemId = systemId;
        }

        public int CustId { get; }

        public int UserId { get; }

        public byte Locked { get; }

        public string IP { get; }

        public byte SystemId { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "Acc_CustomerUpdLocked";

        public bool IsValid()
            => CustId > 0 && UserId > 0;
    }
}

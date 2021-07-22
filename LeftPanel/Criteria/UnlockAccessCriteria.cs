using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    public class UnlockAccessCriteria : CriteriaBase
    {
        public UnlockAccessCriteria(int custId, int userId, byte locked, string ip, byte systemId)
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

        public override string GetSettingKey()
            => "Acc_CustomerUpdLocked";

        public override bool IsValid()
            => CustId > 0 && UserId > 0;
    }
}

using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    public class UnlockAccessCriteria : CriteriaBase
    {
        public UnlockAccessCriteria(int custId, string groupName, int userId, string ip)
        {
            CustId = custId;
            GroupName = groupName;
            UserId = userId;
            IP = ip;
        }

        public int CustId { get; }

        public string GroupName { get; }

        public int UserId { get; }

        public string IP { get; }

        public override string GetSettingKey()
            => "";

        public override bool IsValid()
            => CustId > 0 && UserId > 0;
    }
}

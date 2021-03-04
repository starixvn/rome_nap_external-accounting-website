namespace Fanex.ExternalAccount.HeadlineBonus.Criteria
{
    using Fanex.NAP.Framework.Data;

    internal class CancelPendingBonusCriteria : CriteriaBase
    {
        public CancelPendingBonusCriteria(
            string groupName,
            int custId,
            long transId,
            int userId,
            string ip)
        {
            GroupName = groupName;
            CustId = custId;
            TransId = transId;
            UserId = userId;
            LoginIP = ip;
        }

        public string GroupName { get; }

        public int CustId { get; }

        public long TransId { get; }

        public int UserId { get; }

        public string LoginIP { get; }

        public override string GetSettingKey()
            => "EAS_Bonus_Cancel";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName) && CustId > 0 && TransId > 0;
    }
}

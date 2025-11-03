namespace Fanex.ExternalAccount.HeadlineBonus.Criteria
{
    using Fanex.Data.Repository;
    internal class CancelPendingBonusCriteria : INonQueryCommand
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
        
        public string GetSettingKey() => "EAS_Bonus_Cancel";

        public bool IsValid()  => !string.IsNullOrWhiteSpace(GroupName) && CustId > 0 && TransId > 0;
        
        public int Errcode { get; set; }
        
        public string Errmsg { get; set; }
    }
}

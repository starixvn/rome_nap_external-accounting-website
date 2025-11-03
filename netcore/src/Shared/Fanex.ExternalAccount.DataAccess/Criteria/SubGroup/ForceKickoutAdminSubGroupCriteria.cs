namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.Data.Repository;

    public class ForceKickoutAdminSubGroupCriteria : INonQueryCommand
    {
        public int CustId { get; set; }

        public string GetSettingKey()
            => "EAS_OnUser_ForceKickout";

        public bool IsValid()
            => CustId > 0;
    }
}
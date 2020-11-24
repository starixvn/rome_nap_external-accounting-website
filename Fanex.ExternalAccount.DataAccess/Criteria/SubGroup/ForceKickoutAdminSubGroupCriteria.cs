namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.NAP.Framework.Data;

    public class ForceKickoutAdminSubGroupCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
            => "EAS_OnUser_ForceKickout";

        public override bool IsValid()
            => CustId > 0;
    }
}

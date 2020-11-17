namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.NAP.Framework.Data;

    public class GetSubGroupAdminCriteria : CriteriaBase
    {
        public string SubGroup { get; set; }

        public override string GetSettingKey()
            => "EAS_UserInfoSelBySubGroup";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(SubGroup);
    }
}

namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }
        public byte Mode { get; set; }

        public override string GetSettingKey() => "EAS_GroupInfoSelByGroupID";

        public override bool IsValid() => GroupId > 0;
    }
}
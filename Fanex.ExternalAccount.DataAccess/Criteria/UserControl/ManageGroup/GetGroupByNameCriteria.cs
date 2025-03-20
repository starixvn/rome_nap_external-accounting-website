namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetGroupByNameCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public override string GetSettingKey() => "EAS_GroupInfoSelByGroupName";

        public override bool IsValid() => !string.IsNullOrEmpty(GroupName);
    }
}
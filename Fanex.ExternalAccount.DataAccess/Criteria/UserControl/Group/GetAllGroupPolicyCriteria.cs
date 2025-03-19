namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetAllGroupPolicyCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override string GetSettingKey() => "EAS_GroupPolicySel";

        public override bool IsValid() => GroupId > 0;
    }
}
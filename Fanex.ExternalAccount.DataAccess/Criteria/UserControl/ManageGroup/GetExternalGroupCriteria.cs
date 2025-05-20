namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetExternalGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EAS_GroupInfoSelExternalGroup";

        public override bool IsValid() => true;
    }
}
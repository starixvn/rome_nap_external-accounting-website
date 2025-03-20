namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetListGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EAS_GroupInfoSel";

        public override bool IsValid() => true;
    }
}
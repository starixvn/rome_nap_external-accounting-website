namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Group
{
    using Fanex.Data.Repository;

    public class GetAllGroupCriteria : CriteriaBase
    {
        public int Mode { get; set; }

        public override string GetSettingKey() => "EAS_GroupInfoSelAllGroup";

        public override bool IsValid() => true;
    }
}
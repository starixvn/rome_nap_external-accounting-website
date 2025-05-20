namespace Fanex.ExternalAccount.DataAccess.Criteria.GroupConfiguration
{
    using Fanex.Data.Repository;

    public class GetGroupConfigurationCriteria : CriteriaBase
    {
        public int? ProductId { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey() => "EAS_GroupConfigurationSel";

        public override bool IsValid() => !(GroupId == 0 && ProductId == 0);
    }
}
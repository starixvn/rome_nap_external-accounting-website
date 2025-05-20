namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetExtraPTCriteria : CriteriaBase
    {
        public string GroupName { get; }

        public GetExtraPTCriteria(string groupName)
        {
            GroupName = groupName;
        }

        public override string GetSettingKey() => "EAS_Rpt_ViewExtraPT2";

        public override bool IsValid() => true;
    }
}

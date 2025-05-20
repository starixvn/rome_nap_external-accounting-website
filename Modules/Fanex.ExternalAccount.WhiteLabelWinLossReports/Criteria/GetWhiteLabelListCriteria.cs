namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Criteria
{
    using Fanex.Data.Repository;

    public class GetWhiteLabelListCriteria : CriteriaBase
    {
        public GetWhiteLabelListCriteria(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; }

        public override string GetSettingKey() => "EAS_WhiteLabelListSel";
        
        public override bool IsValid() => !string.IsNullOrWhiteSpace(GroupName);
    }
}
namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Criteria
{
    using Fanex.NAP.Framework.Data;
    
    public class GetWhiteLabelListCriteria : CriteriaBase
    {
        public GetWhiteLabelListCriteria(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; }

        public override string GetSettingKey() => "EAS_WhiteLabelListSel";
    }
}
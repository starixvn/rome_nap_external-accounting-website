namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    using Fanex.Data.Repository;
    public class MatchCriteria : CriteriaBase
    {
        public string GroupName { get; set; }
        
        public string FromDate { get; set;  } 
        
        public int SportType { get; set; }
        
        public string LeagueIds { get; set; }
        
        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_EventSel";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(GroupName) && !string.IsNullOrEmpty(FromDate);
    }
}
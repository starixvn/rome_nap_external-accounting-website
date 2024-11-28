namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class LeaguesCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string GroupName { get; set; }

        public int SportType { get; set; }

        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_LeagueSel";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(GroupName) && !string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate);
    }
}
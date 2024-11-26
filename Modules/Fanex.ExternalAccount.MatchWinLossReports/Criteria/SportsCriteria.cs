namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class SportsCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public int? ProductId { get; set; }

        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_SportSel";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate);
    }
}
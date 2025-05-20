namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using System;
    using Fanex.ExternalAccount.Core.Shared;

    public class ReportRawEntity
    {
        public decimal StakeAvg { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public decimal NetTurnover { get; set; }

        public decimal GrossComm { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal MemberComm { get; set; }


        public decimal MemberMargin { get; set; }

        public decimal MemberTotal { get; set; }

        public decimal AgentWinLoss { get; set; }

        public decimal AgentComm { get; set; }

        public decimal AgentTotal { get; set; }

        public decimal MasterWinLoss { get; set; }

        public decimal MasterComm { get; set; }

        public decimal MasterTotal { get; set; }

        public decimal SuperWinLoss { get; set; }

        public decimal SuperComm { get; set; }

        public decimal SuperTotal { get; set; }

        public decimal CompanyTotal { get; set; }

        public DateTime KickOffTime { get; set; }

        public string KickOffTimeStr  => KickOffTime.ToString();

        public int LeagueId { get; set; }

        public string LeagueCode { get; set; }

        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public int HomeId { get; set; }

        public string HomeName => CacheRef.GetTeamName(HomeId.ToString());

        public int AwayId { get; set; }

        public string AwayName => CacheRef.GetTeamName(AwayId.ToString());

        public string LeagueName => CacheRef.GetLeagueName(LeagueId.ToString());

        public bool IsNeutral { get; set; }

        public int BetTypeId { get; set; }

        public string BetTypeName => CacheRef.GetBettype(BetTypeId.ToString());

        public override bool Equals(object obj)
        {
            if (obj is ReportRawEntity other)
            {
                return StakeAvg == other.StakeAvg &&
                       BetCount == other.BetCount &&
                       Turnover == other.Turnover &&
                       NetTurnover == other.NetTurnover &&
                       GrossComm == other.GrossComm &&
                       MemberWinLoss == other.MemberWinLoss &&
                       MemberComm == other.MemberComm &&
                       MemberMargin == other.MemberMargin &&
                       MemberTotal == other.MemberTotal &&
                       AgentWinLoss == other.AgentWinLoss &&
                       AgentComm == other.AgentComm &&
                       AgentTotal == other.AgentTotal &&
                       MasterWinLoss == other.MasterWinLoss &&
                       MasterComm == other.MasterComm &&
                       MasterTotal == other.MasterTotal &&
                       SuperWinLoss == other.SuperWinLoss &&
                       SuperComm == other.SuperComm &&
                       SuperTotal == other.SuperTotal &&
                       CompanyTotal == other.CompanyTotal &&
                       KickOffTime == other.KickOffTime &&
                       KickOffTimeStr == other.KickOffTimeStr &&
                       LeagueId == other.LeagueId &&
                       LeagueCode == other.LeagueCode &&
                       MatchId == other.MatchId &&
                       MatchCode == other.MatchCode &&
                       HomeId == other.HomeId &&
                       AwayId == other.AwayId;
            }

            return false;
        }
    }
}

namespace Fanex.ExternalAccount.DTO.TotalBet
{
    public class TotalBetModel
    {
        public int LeagueId { get; set; }
        public int MatchId { get; set; }
        public int HomeId { get; set; }
        public int AwayId { get; set; }
        public string ShowTime { get; set; }
        public int LiveIndicator { get; set; }
        public short LiveHomeScore { get; set; }
        public short LiveAwayScore { get; set; }

        public decimal? HDPFT { get; set; }
        public decimal? OUFT { get; set; }
        public decimal? HDPHT { get; set; }
        public decimal? OUHT { get; set; }

        public string HomeName { get; set; }
        public string AwayName { get; set; }

        public bool IsLatest { get; set; }
        public bool IsNeutral { get; set; }

        public int TotalScore => LiveHomeScore + LiveAwayScore;

        public bool Show_FullTime_HDP => HDPFT.HasValue;
        public bool Show_FullTime_OU => OUFT.HasValue;
        public bool Show_1stHalf_HDP => HDPHT.HasValue;
        public bool Show_1stHalf_OU => OUHT.HasValue;
    }
}

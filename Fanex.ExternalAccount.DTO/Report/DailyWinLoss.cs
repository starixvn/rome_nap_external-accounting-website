namespace Fanex.ExternalAccount.DTO.Report
{
    public class DailyWinLoss
    {
        public int No { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }
        
        public int RoleId { get; set; }

        public double TotalTO { get; set; }

        public double ComTO { get; set; }

        public double ComTOPercent { get; set; }

        public double SAgTO { get; set; }

        public double SAgTOPercent { get; set; }

        public double MAgTO { get; set; }

        public double MAgTOPercent { get; set; }

        public double AgTO { get; set; }

        public double AgTOPercent { get; set; }

        public double ComWinlost { get; set; }

        public double ComWinlostPercent { get; set; }

        public double SAgWinlost { get; set; }

        public double SAgWinlostPercent { get; set; }

        public double MAgWinlost { get; set; }

        public double MAgWinlostPercent { get; set; }

        public double AgWinlost { get; set; }

        public double AgWinlostPercent { get; set; }
    }
}
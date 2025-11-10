namespace Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Reports
{
    public class DailyWinLoss
    {
        private const int OneHundred = 100;

        public int No { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public double TotalTO { get; set; }

        public double ComTO { get; set; }

        public double ComTOPercent => TotalTO.Equals(0) ? 0 : ComTO * OneHundred / TotalTO;

        public double SAgTO { get; set; }

        public double SAgTOPercent => TotalTO.Equals(0) ? 0 : SAgTO * OneHundred / TotalTO;

        public double MAgTO { get; set; }

        public double MAgTOPercent => TotalTO.Equals(0) ? 0 : MAgTO * OneHundred / TotalTO;

        public double AgTO { get; set; }

        public double AgTOPercent => TotalTO.Equals(0) ? 0 : AgTO * OneHundred / TotalTO;

        public double ComWinlost { get; set; }

        public double ComWinlostPercent => ComTO.Equals(0) ? 0 : ComWinlost * OneHundred / ComTO;

        public double SAgWinlost { get; set; }

        public double SAgWinlostPercent => SAgTO.Equals(0) ? 0 : SAgWinlost * OneHundred / SAgTO;

        public double MAgWinlost { get; set; }

        public double MAgWinlostPercent => MAgTO.Equals(0) ? 0 : MAgWinlost * OneHundred / MAgTO;

        public double AgWinlost { get; set; }

        public double AgWinlostPercent => AgTO.Equals(0) ? 0 : AgWinlost * OneHundred / AgTO;
    }
}

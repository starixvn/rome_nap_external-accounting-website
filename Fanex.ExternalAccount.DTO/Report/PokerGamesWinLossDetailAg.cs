namespace Fanex.ExternalAccount.DTO.Report
{
    public class PokerGamesWinLossDetailAg
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public double Turnover { get; set; }

        public double NetTurnover { get; set; }

        public double TaxComm { get; set; }

        public double MemberWinLoss { get; set; }

        public double MemberInsuranceGemsWinLoss { get; set; }

        public double MemberComm { get; set; }

        public double MemberTotal { get; set; }

        public double AgInsuranceGemsWinLoss { get; set; }

        public double AgComm { get; set; }

        public double AgTotal { get; set; }

        public double Company { get; set; }
    }
}
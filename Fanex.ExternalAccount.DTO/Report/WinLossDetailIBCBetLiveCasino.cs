namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossDetailIBCBetLiveCasino
    {
        public int CustId { get; set; }
        public string Username { get; set; }
        public double Turnover { get; set; }
        public double NetTurnover { get; set; }
        public double GrossComm { get; set; }
        public double MemberWinlost { get; set; }
        public double PlayerComm { get; set; }
        public double MemberTotal { get; set; }
        public double SAgWinlost { get; set; }
        public double SAgComm { get; set; }
        public double SAgTotal { get; set; }
        public double ExtWinlost { get; set; }
        public double ExtComm { get; set; }
        public double ExtTotal { get; set; }
        public double NetCompany { get; set; }
    }
}
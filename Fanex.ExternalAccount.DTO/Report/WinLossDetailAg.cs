namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossDetailAg
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }
        
        public string Username { get; set; }

        public double Turnover { get; set; }

        public double NetTurnover { get; set; }

        public double GrossComm { get; set; }

        public double MemberWinlost { get; set; }

        public double PlayerComm { get; set; }

        public double MemberTotal { get; set; }

        public double AgWinlost { get; set; }

        public double AgComm { get; set; }

        public double AgTotal { get; set; }

        public double Company { get; set; }
        
        public double RewardAmount { get; set; }
        
        public string RewardCurrency { get; set; }
    }
}
namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossDetailExtra : WinLossDetail
    {
        public double ExtraWinLost { get; set; }

        public double ExtraComm { get; set; }

        public double ExtraTotal { get; set; }

        public double CompanyAfterExtraPT { get; set; }
    }
}
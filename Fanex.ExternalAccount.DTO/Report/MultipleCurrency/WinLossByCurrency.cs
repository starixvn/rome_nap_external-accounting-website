using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossByCurrencyDetail : WinLossDetail
    {
        public string Currency { get; set; }
    }

    public class WinLossByCurrency
    {
        public List<WinLossByCurrencyDetail> WinLossDetails { get; set; }
        public List<WinLossByCurrencyDetail> WinLossTotals { get; set; }
    }
}

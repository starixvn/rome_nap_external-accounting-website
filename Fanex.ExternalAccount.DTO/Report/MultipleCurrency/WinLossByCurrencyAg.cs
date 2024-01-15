using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossByCurrencyDetailAg : WinLossDetailAg
    {
        public string Currency { get; set; }
    }
    public class WinLossByCurrencyAg
    {
        public List<WinLossByCurrencyDetailAg> WinLossDetails { get; set; }
        public List<WinLossByCurrencyDetailAg> WinLossTotals { get; set; }
    }
}

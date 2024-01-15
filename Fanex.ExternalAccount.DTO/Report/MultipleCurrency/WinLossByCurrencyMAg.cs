using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Report
{
    public class WinLossByCurrencyDetailMAg : WinLossDetailMAg
    {
        public string Currency { get; set; }
    }

    public class WinLossByCurrencyMAg
    {
        public List<WinLossByCurrencyDetailMAg> WinLossDetails { get; set; }
        public List<WinLossByCurrencyDetailMAg> WinLossTotals { get; set; }
    }
}

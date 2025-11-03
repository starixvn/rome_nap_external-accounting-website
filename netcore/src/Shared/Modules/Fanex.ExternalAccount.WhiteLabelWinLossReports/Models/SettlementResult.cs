namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Models
{
    using System.Collections.Generic;

    public class SettlementResult
    {
        public SettlementResult(IEnumerable<Settlement> items, Settlement total)
        {
            Items = items;
            RowTotal = total;
        }

        public IEnumerable<Settlement> Items { get; }
        
        public Settlement RowTotal { get; }
    }
}
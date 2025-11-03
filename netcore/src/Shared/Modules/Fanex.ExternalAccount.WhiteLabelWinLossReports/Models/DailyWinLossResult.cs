namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Models
{
    using System.Collections.Generic;

    public class DailyWinLossResult
    {
        public DailyWinLossResult(IEnumerable<DailyWinLoss> items, DailyWinLoss total)
        {
            Items = items;
            RowTotal = total;
        }

        public IEnumerable<DailyWinLoss> Items { get; }

        public DailyWinLoss RowTotal { get; }
    }
}

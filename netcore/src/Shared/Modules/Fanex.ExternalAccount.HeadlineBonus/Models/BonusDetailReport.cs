namespace Fanex.ExternalAccount.HeadlineBonus.Models
{
    using System.Collections.Generic;

    internal class BonusDetailReport
    {
        public BonusDetailReport(IEnumerable<SuperBonus> bonus, decimal totalAmount, int totalItems)
        {
            Bonus = bonus;
            TotalAmount = totalAmount;
            TotalItems = totalItems;
        }

        public IEnumerable<SuperBonus> Bonus { get; }

        public decimal TotalAmount { get; }

        public int TotalItems { get; }
    }
}

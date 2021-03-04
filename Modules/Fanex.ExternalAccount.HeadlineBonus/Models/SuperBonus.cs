namespace Fanex.ExternalAccount.HeadlineBonus.Models
{
    using System.Collections.Generic;

    internal class SuperBonus
    {
        public SuperBonus(string superName, decimal totalAmount, IEnumerable<MemberBonus> bonus)
        {
            SuperName = superName;
            TotalAmount = totalAmount;
            Bonus = bonus;
        }

        public string SuperName { get; }

        public decimal TotalAmount { get; }

        public IEnumerable<MemberBonus> Bonus { get; }
    }
}

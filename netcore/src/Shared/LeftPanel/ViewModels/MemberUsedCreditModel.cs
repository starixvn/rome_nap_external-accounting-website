namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class MemberUsedCreditModel
    {
        public IEnumerable<MemberUsedCreditItem> MemberUsedCreditItems { get; set; }
        public IEnumerable<MemberUsedCreditItem> CashMemberUsedCreditItems { get; set; }

        public IEnumerable<MemberUsedCreditItem> ActiveMemberUsedCreditItems
            => MemberUsedCreditItems?.Count() > 0
                ? MemberUsedCreditItems.Where(o => !o.IsClosed)
                : null;

        public IEnumerable<MemberUsedCreditItem> ActiveCashMemberUsedCreditItems
            => CashMemberUsedCreditItems?.Count() > 0
                ? CashMemberUsedCreditItems.Where(o => !o.IsClosed)
                : null;

        public IEnumerable<MemberUsedCreditItem> InactiveMemberUsedCreditItems
            => MemberUsedCreditItems?.Count() > 0
                ? MemberUsedCreditItems.Where(o => o.IsClosed)
                : null;

        public IEnumerable<MemberUsedCreditItem> InactiveCashMemberUsedCreditItems
            => CashMemberUsedCreditItems?.Count() > 0
                ? CashMemberUsedCreditItems.Where(o => o.IsClosed)
                : null;

        public decimal TotalActiveMemberUsedCredit
            => MemberUsedCreditItems == null || !MemberUsedCreditItems.Any()
                ? 0m
                : MemberUsedCreditItems.Where(x => !x.IsClosed).Sum(o => o.Amount);

        public decimal TotalActiveCashMemberUsedCredit
            => CashMemberUsedCreditItems == null || !CashMemberUsedCreditItems.Any()
                ? 0m
                : CashMemberUsedCreditItems.Where(x => !x.IsClosed).Sum(o => o.Amount);

        public decimal TotalInactiveMemberUsedCredit
            => MemberUsedCreditItems == null || !MemberUsedCreditItems.Any()
                ? 0m
                : MemberUsedCreditItems.Where(x => x.IsClosed).Sum(o => o.Amount);

        public decimal TotalInactiveCashMemberUsedCredit
            => CashMemberUsedCreditItems == null || !CashMemberUsedCreditItems.Any()
                ? 0m
                : CashMemberUsedCreditItems.Where(x => x.IsClosed).Sum(o => o.Amount);
    }
}
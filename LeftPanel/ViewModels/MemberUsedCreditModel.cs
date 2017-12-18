namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class MemberUsedCreditModel
    {
        public IList<MemberUsedCreditItem> MemberUsedCreditItems
        {
            get;
            set;
        }

        public IList<MemberUsedCreditItem> ActiveMemberUsedCreditItems
        {
            get
            {
                if (MemberUsedCreditItems != null && MemberUsedCreditItems.Count > 0)
                {
                    return MemberUsedCreditItems.Where(o => !o.IsClosed).ToList();
                }

                return null;
            }
        }

        public IList<MemberUsedCreditItem> InactiveMemberUsedCreditItems
        {
            get
            {
                if (MemberUsedCreditItems != null && MemberUsedCreditItems.Count > 0)
                {
                    return MemberUsedCreditItems.Where(o => o.IsClosed).ToList();
                }

                return null;
            }
        }

        public decimal TotalActiveMemberUsedCredit
        {
            get
            {
                if (MemberUsedCreditItems == null || MemberUsedCreditItems.Count == 0)
                {
                    return 0m;
                }

                return this.MemberUsedCreditItems.Where(x => !x.IsClosed).Sum(o => o.Amount);                
            }
        }

        public decimal TotalInactiveMemberUsedCredit
        {
            get
            {
                if (MemberUsedCreditItems == null || MemberUsedCreditItems.Count == 0)
                {
                    return 0m;
                }

                return MemberUsedCreditItems.Where(x => x.IsClosed).Sum(o => o.Amount);
            }
        }
    }
}

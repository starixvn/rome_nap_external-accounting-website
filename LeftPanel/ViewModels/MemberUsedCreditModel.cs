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
                if (this.MemberUsedCreditItems != null && this.MemberUsedCreditItems.Count > 0)
                {
                    return this.MemberUsedCreditItems.Where(o => o.IsClosed == false).ToList();
                }

                return null;
            }
        }

        public IList<MemberUsedCreditItem> InactiveMemberUsedCreditItems
        {
            get
            {
                if (this.MemberUsedCreditItems != null && this.MemberUsedCreditItems.Count > 0)
                {
                    return this.MemberUsedCreditItems.Where(o => o.IsClosed == true).ToList();
                }

                return null;
            }
        }

        public decimal TotalActiveMemberUsedCredit
        {
            get
            {
                if (this.MemberUsedCreditItems == null || this.MemberUsedCreditItems.Count == 0)
                {
                    return 0m;
                }

                return this.MemberUsedCreditItems.Where(x => x.IsClosed == false).Sum(o => o.Amount);                
            }
        }

        public decimal TotalInactiveMemberUsedCredit
        {
            get
            {
                if (this.MemberUsedCreditItems == null || this.MemberUsedCreditItems.Count == 0)
                {
                    return 0m;
                }

                return this.MemberUsedCreditItems.Where(x => x.IsClosed == true).Sum(o => o.Amount);
            }
        }
    }
}

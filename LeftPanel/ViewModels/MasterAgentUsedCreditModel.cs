namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    public class MasterAgentUsedCreditModel
    {
        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems { get; set; }

        public MasterAgentUsedCreditItem Total
        {
            get
            {
                if (MasterAgentUsedCreditItems == null || MasterAgentUsedCreditItems.Count == 0)
                {
                    return new MasterAgentUsedCreditItem();
                }

                var givenCredit = 0m;
                var usedCredit = 0m;

                foreach (var item in MasterAgentUsedCreditItems)
                {
                    givenCredit += item.GivenCredit;
                    usedCredit += item.UsedCredit;
                }

                return new MasterAgentUsedCreditItem
                {
                    GivenCredit = givenCredit,
                    UsedCredit = usedCredit
                };
            }
        }
        
    }

 
}
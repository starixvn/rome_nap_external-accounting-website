// --------------------------------------------------------------------------
// <copyright file="MasterAgentUsedCreditModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.26">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MasterAgentUsedCreditModel
    {
        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems
        {
            get;
            set;
        }

        public MasterAgentUsedCreditItem Total
        {
            get
            {
                if (this.MasterAgentUsedCreditItems == null || this.MasterAgentUsedCreditItems.Count == 0)
                {
                    return new MasterAgentUsedCreditItem();
                }

                decimal givenCredit = 0m;
                decimal usedCredit = 0m;

                foreach (var item in this.MasterAgentUsedCreditItems)
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

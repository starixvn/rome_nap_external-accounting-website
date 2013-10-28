// --------------------------------------------------------------------------
// <copyright file="MasterAgentUsedCreditItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.26">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MasterAgentUsedCreditItem
    {
        public string Username
        {
            get;
            set;
        }

        public decimal GivenCredit
        {
            get;
            set;
        }

        public decimal UsedCredit
        {
            get;
            set;
        }
    }
}
// --------------------------------------------------------------------------
// <copyright file="BalanceInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Balance information object.
    /// </summary>
    public class BalanceInfo
    {
        public BalanceInfo()
        {
            this.Balances = new List<ObjectInfo>();
            this.Statuses = new List<Status>();
        }

        public ObjectInfo CurrencyId { get; set; }

        public ObjectInfo Currency { get; set; }

        public List<ObjectInfo> Balances { get; set; }

        public List<Status> Statuses { get; set; }
    }
}

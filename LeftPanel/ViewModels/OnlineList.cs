// --------------------------------------------------------------------------
// <copyright file="OnlineList.cs" company="Nexcel Solutions Vietnam">
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
    /// Online list object.
    /// </summary>
    public class OnlineList
    {
        public OnlineList()
        {
            this.Onlines = new List<OnlineInfo>();
        }

        public List<OnlineInfo> Onlines { get; set; }
    }
}

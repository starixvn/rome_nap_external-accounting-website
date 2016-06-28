// --------------------------------------------------------------------------
// <copyright file="DownlineList.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Down-line list object.
    /// </summary>
    public class DownlineList : BasicInfo
    {
        public DownlineList()
        {
            this.Downlines = new List<ObjectInfo>();
            this.Pages = new List<ObjectInfo>();
            this.CurrentPage = 1;
        }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}

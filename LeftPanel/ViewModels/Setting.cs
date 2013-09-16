// --------------------------------------------------------------------------
// <copyright file="Setting.cs" company="Nexcel Solutions Vietnam">
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
    /// Setting object.
    /// </summary>
    public class Setting
    {
        public Setting()
        {
            this.EnableItems = new List<ObjectInfo>();
            this.EditItems = new List<ObjectInfo>();
            this.SyncItems = new List<ObjectInfo>();
            this.Details = new List<ObjectInfo>();
        }

        public List<ObjectInfo> EnableItems { get; set; }

        public List<ObjectInfo> EditItems { get; set; }

        public List<ObjectInfo> SyncItems { get; set; }

        public List<ObjectInfo> Details { get; set; }
    }
}

// --------------------------------------------------------------------------
// <copyright file="Status.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// Status object.
    /// </summary>
    public class Status
    {
        public int TotalActive { get; set; }

        public int TotalClosed { get; set; }

        public int TotalSuspended { get; set; }

        public string Text { get; set; }

        public string Tooltip { get; set; }
    }
}

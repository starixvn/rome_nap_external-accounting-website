// --------------------------------------------------------------------------
// <copyright file="ProductStatuses.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.10.14">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductStatuses
    {
        public ProductStatuses UplineStatuses
        {
            get;
            set;
        }

        public int RoleId
        {
            get;
            set;
        }

        public bool SyncBingo
        {
            get;
            set;
        }

        public bool SyncCasino
        {
            get;
            set;
        }

        public bool SyncLiveCasino
        {
            get;
            set;
        }

        public bool AllowDoubleComm
        {
            get;
            set;
        }

        public bool DisableTransfer
        {
            get;
            set;
        }

        public bool DisableVirtualSports
        {
            get;
            set;
        }

        public bool AllowOutright
        {
            get;
            set;
        }

        public bool DisableNumberGame
        {
            get;
            set;
        }

        public bool DisableCasino
        {
            get;
            set;
        }

        public bool DisableBingo
        {
            get;
            set;
        }

        public bool DisableLiveCasino
        {
            get;
            set;
        }

        public bool Closed
        {
            get;
            set;
        }

        public bool Suspended
        {
            get;
            set;
        }

        public bool HideVirtualSports
        {
            get;
            set;
        }

        public bool DisableHRFixedOdds
        {
            get;
            set;
        }
    }
}
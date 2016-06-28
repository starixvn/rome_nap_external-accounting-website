// --------------------------------------------------------------------------
// <copyright file="MemberUsedCreditItem.cs" company="">
// Copyright (c) . All rights reserved.
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
    public class MemberUsedCreditItem
    {
        public string Username
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public decimal Amount
        {
            get;
            set;
        }

        public bool IsClosed
        {
            get;
            set;
        }
    }
}
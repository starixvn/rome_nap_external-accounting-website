// --------------------------------------------------------------------------
// <copyright file="CustInfo.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Customer Information
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.5">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.Customer
{
    /// <summary>
    /// Customer Information.
    /// </summary>
    public class CustInfo
    {
        public int CustId
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public int RoleId
        {
            get;
            set;
        }

        public int SuperId
        {
            get;
            set;
        }

        public int MasterId
        {
            get;
            set;
        }

        public int AgentId
        {
            get;
            set;
        }

        public string SuperName
        {
            get;
            set;
        }

        public string MasterName
        {
            get;
            set;
        }

        public string AgentName
        {
            get;
            set;
        }
    }
}

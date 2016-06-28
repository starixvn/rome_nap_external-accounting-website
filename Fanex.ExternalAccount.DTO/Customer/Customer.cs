// --------------------------------------------------------------------------
// <copyright file="Customer.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.DTO.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Customer
    {
        public int CustId
        {
            get;
            set;
        }

        public int RoleId
        {
            get;
            set;
        }

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

        public string MobilePhone
        {
            get;
            set;
        }

        public string HomePhone
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Fax
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }
    }
}
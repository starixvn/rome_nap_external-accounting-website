// --------------------------------------------------------------------------
// <copyright file="IPInfo.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.1.23">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.Customer
{
    /// <summary>
    /// IP information.
    /// </summary>
    public class IPInfo
    {
        public string IP { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Flag { get; set; }

        public string Info { get; set; }
    }
}
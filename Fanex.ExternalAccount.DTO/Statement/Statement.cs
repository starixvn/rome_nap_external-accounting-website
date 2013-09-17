// --------------------------------------------------------------------------
// <copyright file="Statement.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DTO.Statement
{
    using System;

    /// <summary>
    /// TODO: Summary description.
    /// </summary>
    public class Statement
    {
        public string AccDate { get; set; }

        public long RefNo { get; set; }

        public double TotalAmt { get; set; }

        public string TransType { get; set; }

        public string TransDesc { get; set; }
    }
}
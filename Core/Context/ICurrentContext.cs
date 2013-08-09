// --------------------------------------------------------------------------
// <copyright file="ICurrentContext.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Context
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    public interface ICurrentContext
    {
        HttpContext Context { get; }

        string Ip4Address { get; }

        string Ip6Address { get; }

        string DomainName { get; }

        string FullDomainName { get; }

        string CountryCode { get; }
    }
}
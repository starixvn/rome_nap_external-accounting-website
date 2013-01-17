// --------------------------------------------------------------------------
// <copyright file="IAgentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.28">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IAgentService
    {
        DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);
    }
}
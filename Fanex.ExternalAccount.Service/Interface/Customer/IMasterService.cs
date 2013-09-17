// --------------------------------------------------------------------------
// <copyright file="IMasterService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.28">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMasterService
    {
        DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);

        Transfer GetTransferOption(int custId);

        Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language);
    }
}
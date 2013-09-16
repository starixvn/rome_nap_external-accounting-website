// --------------------------------------------------------------------------
// <copyright file="ILeftPanelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Interface
{
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILeftPanelService
    {
        CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1);

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DataTable GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);
    }
}

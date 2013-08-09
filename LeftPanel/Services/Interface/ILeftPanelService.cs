// --------------------------------------------------------------------------
// <copyright file="ILeftPanelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.CustomerSetting.Module.LeftPanel.Services.Interface
{
    using System.Data;
    using Fanex.CustomerSetting.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILeftPanelService
    {
        CustomerModel GetCustomerInfo(string username);

        Result Update(int id, int uplineId, CustomerModel model);

        DataTable GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);
    }
}

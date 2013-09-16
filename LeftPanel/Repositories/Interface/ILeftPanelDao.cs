// --------------------------------------------------------------------------
// <copyright file="ILeftPanelDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface
{
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILeftPanelDao
    {
        DataSet GetCustomerInfo(string username, string groupName = "ACCDDD");

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DbResult GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);
    }
}

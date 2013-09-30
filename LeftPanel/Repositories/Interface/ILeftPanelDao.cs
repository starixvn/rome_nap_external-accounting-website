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
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILeftPanelDao
    {
        DataSet GetCustomerInfo(string username, string groupName = "ACCDDD");

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DbResult GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        int UserGroupId(int custId);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        DataTable CustomerSelectClearBalance(int custId);
    }
}

namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using Criteria;
    using Data.DNAide;
    using DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Models;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

#pragma warning disable IDE1006 // Naming Styles
    public interface CustomerDataAccess
#pragma warning restore IDE1006 // Naming Styles
    {
        DataSet GetPersonalInfo(string username, string groupName, int userId);

        DataSet GetLoginInfo(int custId, string groupName);

        DataSet GetCustomerStatusInfo(int custId, byte settingStatus);

        DataSet GetBalanceInfo(int custId);

        DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        DbResult GetDownlineList(int custid, int pageSize, int pageIndex);

        DataRowCollection GetOnlineList(int custid);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int role);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        DataTable CustomerSelectClearBalance(int custId);

        Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        Result AllowOutright(int custId, bool allow);

        Result AutoMasterPT(int id, bool allow);

        Result ResetSecurityCode(int id, string code, bool force);        

        Result RemoveOTP(RemoveOtpCriteria criteria);

        MemberBalanceInfoModel GetMemberBalanceModel(int custId);

        decimal GetMemberOustanding(int custId);
    }
}
namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using System.Collections.Generic;
    using System.Data;
    using Criteria;
    using DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Models;
    using ViewModels;

    public interface CustomerDataAccess
    {
        DataSet GetPersonalInfo(GetPersonalInfoCriteria criteria);

        DataSet GetLoginInfo(int custId, string groupName);

        DataSet GetCustomerStatusInfo(int custId, byte settingStatus);

        DataSet GetBalanceInfo(int custId);

        Result UpdateSMAGivenCredit(int id, int uplineId, decimal newGivenCredit);

        Result UpdateMemberGivenCredit(int id, int uplineId, decimal newGivenCredit, decimal totalAmt);

        DataSet GetDownlineList(bool isDirectDownline, int custid, int pageSize, int pageIndex, out int totalPages);

        DataSet GetOnlineList(int custid);

        IEnumerable<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int role);

        IEnumerable<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        Result ResetMemberCredit(int custId, int uplineId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip, SiteDB siteDB);

        DataTable CustomerSelectClearBalance(int custId);

        Result CloseCustomer(int custId, bool closed, bool clearCredit, string custName);

        Result UnlockAccess(UnlockAccessCommand command);

        Result AllowOutright(int custId, bool allow);

        Result AutoMasterPT(int id, bool allow);

        Result ResetSecurityCode(int id, string code, bool force);        

        Result RemoveOTP(RemoveOtpCommand criteria);

        MemberBalanceInfoModel GetMemberBalanceModel(int custId, SiteDB siteDB);

        decimal GetMemberOustanding(int custId);

        decimal GetTotalAmount(int custId, SiteDB siteDB);

        Result ClearMemberCredit(decimal totalAmt, string username = null);

        Result ClearSMACredit(string username = null);
    }
}
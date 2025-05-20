namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using Core.Authorization;
    using Customers;
    using DataAccesses;
    using Extensions;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Models;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

#pragma warning disable IDE1006 // Naming Styles
    public interface CustomerQueryService
#pragma warning restore IDE1006 // Naming Styles
    {
        CustomerModel SearchCustomer(int actorId, string custName, string groupName, byte settingStatus, Result result);

        OnlineList GetOnlineList(int custId);

        DownlineList GetDownlineList(bool isDirectDownline, int custid, int pageSize, int pageIndex);

        IEnumerable<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IEnumerable<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        DataTable CustomerSelectClearBalance(int custId);
    }

    public class CustomerQueryServiceImpl : CustomerQueryService
    {
        private readonly AuthorizationService authorizationService;
        private readonly CustomerDataAccess customerDataAccess;
        private readonly CustomerInfoQueryService customerInfoQueryService;
        private readonly ISiteDBConfigService siteDBService;

        public CustomerQueryServiceImpl(
            CustomerDataAccess customerDataAccess = null,
            AuthorizationService authorizationService = null,
            CustomerInfoQueryService customerInfoQueryService = null,
            ISiteDBConfigService siteDBService = null)
        {
            this.customerDataAccess = customerDataAccess ?? new CustomerDataAccessImpl();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.customerInfoQueryService = customerInfoQueryService ?? new CustomerInfoQueryServiceImpl();
            this.siteDBService = siteDBService ?? new SiteDBConfigService();
        }

        public CustomerModel SearchCustomer(int actorId, string custName, string groupName, byte settingStatus, Result result)
        {
            var getPersonalInfoCriteria = new Criteria.GetPersonalInfoCriteria(
                string.IsNullOrWhiteSpace(custName)
                    ? string.Empty
                    : custName,
                groupName,
                actorId);
            var personalInfoDataSet = customerDataAccess.GetPersonalInfo(getPersonalInfoCriteria);

            if (personalInfoDataSet == null || personalInfoDataSet.Tables.Count <= 0)
            {
                result.ErrorCode = getPersonalInfoCriteria.ErrCode.ToString();
                result.ErrorMessage = getPersonalInfoCriteria.ErrCode > 0 ? getPersonalInfoCriteria.ErrMsg : "There is no information.";
                return null;
            }

            var custId = personalInfoDataSet.Tables[0].Rows[0].CustId();
            var roleId = personalInfoDataSet.Tables[0].Rows[0].RoleId();
            var memberBalanceInfoModel = new MemberBalanceInfoModel();
            if(roleId == Role.Member)
            {
                var siteDB = siteDBService.GetSiteDBConfigByCustId(custId);
                memberBalanceInfoModel = customerDataAccess.GetMemberBalanceModel(custId, siteDB);
            }

            return customerInfoQueryService.GetCustomerModel(
                actorId, 
                custName, 
                groupName,
                personalInfoDataSet,
                customerDataAccess.GetLoginInfo(custId, groupName),
                customerDataAccess.GetBalanceInfo(custId),
                customerDataAccess.GetCustomerStatusInfo(custId, settingStatus),
                memberBalanceInfoModel.Balance,
                memberBalanceInfoModel.PendingTransfer3rd,
                roleId == Role.Member ? customerDataAccess.GetMemberOustanding(custId) : 0);
        }

        public OnlineList GetOnlineList(int custId)
        {
            if (Config.IsEnabledRecheckin)
            {
                return new OnlineList(customerDataAccess.GetOnlineList(custId), authorizationService.CanViewVNIP);
            }

            return new OnlineList();
        }

        public DownlineList GetDownlineList(bool isDirectDownline, int custid, int pageSize, int pageIndex)
        {
            var downlineListDataSet = customerDataAccess.GetDownlineList( isDirectDownline,custid, pageSize, pageIndex, out int totalPages);
            return new DownlineList(downlineListDataSet, pageIndex, totalPages);
        }

        public IEnumerable<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId)
            => customerDataAccess.MasterAgentUsedCreditItems(custId, roleId);

        public IEnumerable<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
            => customerDataAccess.MemberUsedCreditItems(custId);

        public DataTable CustomerSelectClearBalance(int custId)
            => customerDataAccess.CustomerSelectClearBalance(custId);
    }
}
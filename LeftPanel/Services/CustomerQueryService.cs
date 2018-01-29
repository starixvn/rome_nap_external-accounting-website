namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Customers;
    using DataAccesses;
    using Extensions;
    using Utilities;
    using ViewModels;

    public interface CustomerQueryService
    {
        CustomerModel SearchCustomer(int actorId, string custName, string groupName = "ACCDDD");

        OnlineList GetOnlineList(int custId);

        DownlineList GetDownlineList(int custid, int pageSize, int pageIndex);

        IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        DataTable CustomerSelectClearBalance(int custId);
    }

    public class CustomerQueryServiceImpl : CustomerQueryService
    {
        private readonly AuthorizationService authorizationService;
        private readonly CustomerDataAccess customerDataAccess;
        private readonly CustomerInfoQueryService customerInfoQueryService;

        public CustomerQueryServiceImpl(
            CustomerDataAccess customerDataAccess = null,
            AuthorizationService authorizationService = null,
            CustomerInfoQueryService customerInfoQueryService = null)
        {
            this.customerDataAccess = customerDataAccess ?? new CustomerDataAccessImpl();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.customerInfoQueryService = customerInfoQueryService ?? new CustomerInfoQueryServiceImpl();
        }

        public CustomerModel SearchCustomer(int actorId, string custName, string groupName = "ACCDDD")
        {
            var customerInfoDataSet = customerDataAccess.GetCustomerInfo(
                string.IsNullOrWhiteSpace(custName)
                    ? string.Empty
                    : custName,
                groupName);

            if (customerInfoDataSet == null || customerInfoDataSet.Tables.Count <= 0)
            {
                return null;
            }

            return customerInfoQueryService.GetCustomerModel(actorId, custName, groupName, customerInfoDataSet);
        }

        public OnlineList GetOnlineList(int custId)
            => new OnlineList(customerDataAccess.GetOnlineList(custId), authorizationService.CanViewVNIP);

        public DownlineList GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            var dr = customerDataAccess.GetDownlineList(custid, pageSize, pageIndex);

            if (dr?.ResultSet == null || dr.ResultSet.Tables.Count <= 0)
            {
                return null;
            }

            var downlines = dr.ResultSet.Tables[0];
            var model = new DownlineList();

            if (downlines?.Rows.Count <= 0)
            {
                return model;
            }

            DownlineList.AddDownlines(downlines, model);

            model.CurrentPage = pageIndex;

            var totalPages = Utils.ToInt32(dr.Outputs["totalpages"]);
            DownlineList.AddPageItems(totalPages, model);

            return model;
        }

        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId)
            => customerDataAccess.MasterAgentUsedCreditItems(custId, roleId);

        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
            => customerDataAccess.MemberUsedCreditItems(custId);

        public DataTable CustomerSelectClearBalance(int custId)
            => customerDataAccess.CustomerSelectClearBalance(custId);
    }
}
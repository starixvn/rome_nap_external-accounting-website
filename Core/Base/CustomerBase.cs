namespace Fanex.ExternalAccount.Core.Base
{
    using Module.NG2.Entities;
    using Module.NG2.Services;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.Products;
    using System;
    using Utilities;
    using ViewModel.Customer;

    public class CustomerBase : BaseController
    {
        private readonly IReportService _report = new ReportService();
        protected readonly ProductStatusService productStatusService = new ProductStatusServiceImpl();


        protected Module.NG2.Entities.Result AutoInsertNG2Product(int custId, string username, RoleId roleId,
            int uplineId, SiteId siteId, int currencyId, byte settingStatus)
        {
            var dcsStatuses = productStatusService.GetProductStatus(custId, (int)roleId, 23, settingStatus);

            if (dcsStatuses.IsSyncedUpline)
            {
                ING2StatusService service = new NG2StatusService();
                return service.AutoInsertAccount(custId, username, roleId, uplineId, dcsStatuses.IsDisabledUpline,
                    siteId, currencyId, CurrentSession.UserId, (byte)Config.SystemId);
            }

            return new Module.NG2.Entities.Result();
        }

        protected CustInfo GetCustomerInfo(int custId, string groupName)
        {
            var custInfo = _report.GetCustomerInfo(groupName, custId, string.Empty, true);

            if (custInfo == null || custInfo.Rows.Count == 0)
            {
                return new CustInfo();
            }

            var item = custInfo.Rows[0];

            return new CustInfo
            {
                AgentId = (int)item["recommend"],
                MasterId = (int)item["mrecommend"],
                SuperId = (int)item["srecommend"],
                AgentName = item["agentname"] is DBNull ? string.Empty : item["agentname"].ToString(),
                MasterName = item["mastername"] is DBNull ? string.Empty : item["mastername"].ToString(),
                SuperName = item["supername"] is DBNull ? string.Empty : item["supername"].ToString(),
                CustId = (int)item["custid"],
                Username = (string)item["username"],
                RoleId = Utils.ToInt32(item["roleid"])
            };
        }
    }
}
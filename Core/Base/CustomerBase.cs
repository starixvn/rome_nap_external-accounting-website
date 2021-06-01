namespace Fanex.ExternalAccount.Core.Base
{
    using CasinoLibrary.Model.UserException;
    using Logging.Extensions.Web;
    using Module.NG2.Entities;
    using Module.NG2.Services;
    using Service.Impl.Customer.Casino;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.Products;
    using System;
    using System.Diagnostics;
    using System.Web.Mvc;
    using Utilities;
    using ViewModel.Customer;
    using Customer = CasinoLibrary.SBAgent.Customer;
    using Result = CustomerSetting.Module.Core.Models.Result;

    public class CustomerBase : BaseController
    {
        private const string CasinoNo = "01";
        private const int ErrorCode_Error = 999;
        private readonly IReportService _report = new ReportService();
        protected readonly ProductStatusService productStatusService = new ProductStatusServiceImpl();

        [NonAction]
        protected Result AutoInsertCasinoPost(int custId, int uplineId)
        {
            try
            {
                var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

                if (custInfo?.UplineCasinoSync == true && !custInfo.CasinoSync)
                {
                    var isSuccess = new Customer().AutoInsertAccount(
                        CasinoNo,
                        custInfo.IBCCustID,
                        custInfo.UserName,
                        custInfo.UserName2,
                        string.Empty,
                        string.Empty,
                        custInfo.UserPwd,
                        custInfo.Phone,
                        custInfo.MobilePhone,
                        custInfo.Currency,
                        custInfo.Fax,
                        custInfo.Closed,
                        custInfo.Suspend,
                        custInfo.IsEnablePT,
                        custInfo.SIBCCustID,
                        custInfo.MIBCCustID,
                        custInfo.AIBCCustID,
                        custInfo.RoleId.ToString(),
                        false,
                        false,
                        !custInfo.IsDisableAutoMPT,
                        DateTime.Now,
                        CurrentContext.Ip4Address,
                        uplineId,
                        CurrentSession.UserName,
                        CurrentSession.FromSystem);

                    if (isSuccess)
                    {
                        new CasinoManager().UpdateCSStatus(custId, true, custInfo.CasinoDisabled, CurrentSession.UserId,
                            CurrentContext.Ip4Address, 0, "E");
                    }
                    else
                    {
                        new Customer().RemoveAccount(custInfo.UserName, CurrentContext.Ip4Address,
                            CurrentSession.UserId, CurrentSession.UserName, CurrentSession.FromSystem);
                    }
                }

                return new Result(0, string.Empty);
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
                return new Result(ErrorCode_Error, casinoEx.Message);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return new Result(ErrorCode_Error, string.Empty);
        }

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
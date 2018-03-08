namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CasinoLibrary.Model.UserException;
    using Data;
    using Data.DNAide;
    using LiveCasino.CoreSetting.Services;
    using Logging.Extensions.Web;
    using Module.NG2.Entities;
    using Module.NG2.Services;
    using Service.Impl.Customer.Casino;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.Products;
    using Utilities;
    using ViewModel.Customer;
    using Customer = CasinoLibrary.SBAgent.Customer;
    using Result = CustomerSetting.Module.Core.Models.Result;

    public class CustomerBase : BaseController
    {
        private const string CasinoNo = "01";
        private readonly IReportService _report = new ReportService();
        protected readonly ProductStatusService productStatusService = new ProductStatusServiceImpl();

        [NonAction]
        protected Task<Result> AutoInsertLiveCasinoPost(
            int custId,
            int parentId,
            int actorId,
            string actorUsername,
            string ip)
        {
            return Task.Factory.StartNew(() =>
            {
                var livecasinoSettingService = new SettingService();

                var customerInfo = livecasinoSettingService.GetCustomerLiveCasinoInfo(custId);
                var parentInfo = livecasinoSettingService.GetCustomerLiveCasinoInfo(parentId);

                if (!customerInfo.IsSyncLiveCasino && parentInfo.IsSyncLiveCasino)
                {
                    Thread.Sleep(5000);

                    var results = livecasinoSettingService.AutoInsertAccount(
                        custId,
                        actorId,
                        actorUsername,
                        0,
                        string.Empty,
                        ip,
                        "E");

                    return new Result(results.Code, results.Message);
                }

                return new Result(0, string.Empty);
            });
        }

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
                        custInfo.FirstName,
                        custInfo.LastName,
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
                        FromSystem);

                    if (isSuccess)
                    {
                        new CasinoManager().UpdateCSStatus(custId, true, custInfo.CasinoDisabled, CurrentSession.UserId,
                            CurrentContext.Ip4Address, 0, "E");
                    }
                    else
                    {
                        new Customer().RemoveAccount(custInfo.UserName, CurrentContext.Ip4Address,
                            CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                    }
                }

                return new Result(0, string.Empty);
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
                return new Result(999, casinoEx.Message);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return new Result(999, string.Empty);
        }

        protected Module.NG2.Entities.Result AutoInsertNG2Product(int custId, string username, RoleId roleId,
            int uplineId, SiteId siteId, int currencyId)
        {
            var dcsStatuses = productStatusService.GetProductStatus(custId, (int)roleId, 23);

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
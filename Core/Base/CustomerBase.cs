namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CasinoLibrary.Model.UserException;
    using Data.DNAide;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasino.CoreSetting.Services;
    using Logging.Extensions.Web;
    using Module.NG2.Entities;
    using Module.NG2.Services;

    public class CustomerBase : BaseController
    {
        private const string CasinoNo = "01";

        private readonly IReportService _report = new ReportService();

        #region Live Casino

        [NonAction]
        protected Task<CustomerSetting.Module.Core.Models.Result> AutoInsertLiveCasinoPost(
            int custId,
            int parentId,
            int actorId,
            string actorUsername,
            string ip)
        {
            return Task.Factory.StartNew(() =>
            {
                ISettingService livecasinoSettingService = new SettingService();

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

                    return new CustomerSetting.Module.Core.Models.Result(results.Code, results.Message);
                }

                return new CustomerSetting.Module.Core.Models.Result(0, string.Empty);
            });
        }

        #endregion

        #region Casino

        [NonAction]
        protected CustomerSetting.Module.Core.Models.Result AutoInsertCasinoPost(int custId, int uplineId)
        {
            try
            {
                var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

                if (custInfo?.UplineCasinoSync == true && !custInfo.CasinoSync)
                {
                    var isSuccess = new CasinoLibrary.SBAgent.Customer().AutoInsertAccount(
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
                        new CasinoManager().UpdateCSStatus(custId, true, custInfo.CasinoDisabled, CurrentSession.UserId, CurrentContext.Ip4Address, 0, "E", string.Empty, string.Empty);
                    }
                    else
                    {
                        new CasinoLibrary.SBAgent.Customer().RemoveAccount(custInfo.UserName, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                    }
                }

                return new CustomerSetting.Module.Core.Models.Result(0, string.Empty);
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx));
                return new CustomerSetting.Module.Core.Models.Result(999, casinoEx.Message);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return new CustomerSetting.Module.Core.Models.Result(999, string.Empty);
        }

        #endregion

        protected Module.NG2.Entities.Result AutoInsertNG2Product(int custId, string username, RoleId roleId, int uplineId, SiteId siteId, int currencyId)
        {
            var dcsStatuses = GetDcsStatuses(custId, (int)roleId, 23);

            if (dcsStatuses.IsSyncedUpline)
            {
                ING2StatusService service = new NG2StatusService();
                return service.AutoInsertAccount(custId, username, roleId, uplineId, dcsStatuses.IsDisabledUpline, siteId, currencyId, CurrentSession.UserId, (byte)Config.SystemId);
            }

            return new Module.NG2.Entities.Result();
        }

        protected CustInfo GetCustomerInfo(int custId, string groupName)
        {
            var custInfo = this._report.GetCustomerInfo(groupName, custId, string.Empty, true);

            if (custInfo == null || custInfo.Rows.Count == 0)
            {
                return new CustInfo();
            }

            var item = custInfo.Rows[0];
            var info = new CustInfo
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

            return info;
        }

        protected DcsEntity GetDcsStatuses(int custId, int roleId, int productId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("roleid", roleId);

            var ds = DbHelper.ExecuteDataSet("Acc_CustSetting_StatusSettingSelect", param);

            if (ds != null && ds.Tables.Count > 0)
            {
                var cust = ds.Tables[0];
                var product = cust.Select("ProductID = " + productId).FirstOrDefault();
                if (product != null)
                {
                    var isSyncedUpline = product.ToBoolean("IsSyncedUpline");
                    var isDisabledUpline = product.ToBoolean("IsDisabledUpline");
                    var isHidden = product.ToBoolean("IsHidden");
                    var isSynced = product.ToBoolean("IsSync");
                    var isDisabled = product.ToBoolean("IsDisabled");
                    var isDisabledDownline = product.ToBoolean("IsDisabledDownline");
                    var hasDownline = product.ToBoolean("HasDownline");

                    return new DcsEntity
                    {
                        IsDisabledUpline = isDisabledUpline,
                        IsSyncedUpline = isSyncedUpline,
                        IsHidden = isHidden,
                        HasDownline = hasDownline,
                        IsSynced = isSynced,
                        IsDisabled = isDisabled,
                        IsDisabledDownline = isDisabledDownline
                    };
                }
            }

            return new DcsEntity() { IsHidden = true };
        }
    }
}

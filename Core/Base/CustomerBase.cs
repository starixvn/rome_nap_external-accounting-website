// --------------------------------------------------------------------------
// <copyright file="CustomerBase.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.10">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Service.Impl.Customer.Bingo;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasino.CoreSetting.Services;

    /// <summary>
    /// Base class for customer controller.
    /// </summary>
    public class CustomerBase : BaseController
    {
        private const string CasinoNo = "01";

        private readonly IReportService _report = new ReportService();

        #region Live Casino

        /// <summary>
        /// Auto insert live casino.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Upper id of customer.</param>
        /// <param name="uplineRoleId">Upper role id of customer.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>ActionResult object.</returns>
        public ActionResult AutoInsertLiveCasinoPost(int custId, int parentId, int uplineRoleId, string groupName)
        {
            SettingService livecasinoSettingService = new SettingService();
            var customerInfo = livecasinoSettingService.GetCustomerLiveCasinoInfo(custId);
            var parentInfo = livecasinoSettingService.GetCustomerLiveCasinoInfo(parentId);

            if (!customerInfo.IsSyncLiveCasino && parentInfo.IsSyncLiveCasino)
            {
                System.Threading.Thread.Sleep(5000);
                var results = livecasinoSettingService.AutoInsertAccount(
                                                                    custId,
                                                                    this.CurrentSession.UserId,
                                                                    this.CurrentSession.UserName,
                                                                    0,
                                                                    string.Empty,
                                                                    this.CurrentContext.Ip4Address,
                                                                    "E");

                return this.JsonBase(new { ErrorCode = results.Code, ErrorMessage = results.Message });
            }

            return this.JsonBase(new { ErrorCode = 0, ErrorMessage = string.Empty });
        }

        #endregion

        #region Casino

        /// <summary>
        /// Auto insert casino.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="uplineName">Upper name of customer.</param>
        /// <returns>ActionResult object.</returns>
        public ActionResult AutoInsertCasinoPost(int custId, int uplineId, string uplineName)
        {
            try
            {
                var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                if (custInfo != null && custInfo.UplineCasinoSync && !custInfo.CasinoSync)
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
                        custInfo.IsDisableAutoMPT,
                        DateTime.Now,
                        CurrentContext.Ip4Address,
                        uplineId,
                        CurrentSession.UserName,
                        BaseController.FromSystem);
                    if (isSuccess)
                    {
                        new CasinoManager().UpdateCSStatus(custId, true, custInfo.CasinoDisabled, CurrentSession.UserId, CurrentContext.Ip4Address, 0, "E", string.Empty, string.Empty);
                        this.UpdateCasinoStatus2(custId, custInfo.Closed, custInfo.Suspend);
                    }                    
                }

                return this.JsonBase(new { ErrorCode = 0, ErrorMessage = string.Empty });
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
                return this.JsonBase(new { ErrorCode = 999, ErrorMessage = string.Empty });
            }            
        }

        #endregion

        #region Bingo

        /// <summary>
        /// Auto insert bingo.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="uplineName">Upper name of customer.</param>
        /// <returns>ActionResult object.</returns>
        public ActionResult AutoInsertBingoPost(int custId, int uplineId, string uplineName)
        {
            var isSuccess = false;
            var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (custInfo != null && custInfo.UplineBingoSync && !custInfo.BingoSync)
            {
                LiveCasinoLibrary.SBAgent.Customer customer = new LiveCasinoLibrary.SBAgent.Customer();

                try
                {
                    isSuccess = customer.AutoInsertAccount(
                        CasinoNo,
                        custInfo.IBCCustID,
                        custInfo.UserName,
                        custInfo.SIBCCustID,
                        custInfo.MIBCCustID,
                        custInfo.AIBCCustID,
                        custInfo.RoleId.ToString(),
                        custInfo.Currency,
                        DateTime.Now,
                        this.CurrentContext.Ip4Address,
                        uplineId,
                        CurrentSession.UserName,
                        BaseController.FromSystem);
                }
                catch (Exception e)
                {
                    Logs.Error(e);
                }
            }

            if (isSuccess)
            {
                var err = string.Empty;
                var result = BingoManager.Instance.UpdateBingoStatus(
                    custId, 
                    this.CurrentSession.UserId, 
                    0, 
                    true, 
                    custInfo.BingoDisabled, 
                    true, 
                    this.CurrentContext.Ip4Address, 
                    "En", 
                    ref err);

                return this.JsonBase(new { ErrorCode = result, ErrorMessage = err });
            }

            return this.JsonBase(new { ErrorCode = 999, ErrorMessage = string.Empty });
        }

        #endregion

        /// <summary>
        /// Gets customer info.
        /// </summary>
        /// <param name="custId">Customer Id.</param>
        /// <param name="groupName">Group Name that user belongs to.</param>
        /// <returns>CUSTINFO object.</returns>
        private CustInfo GetCustomerInfo(int custId, string groupName)
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
    }
}

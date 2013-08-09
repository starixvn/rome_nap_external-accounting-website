// --------------------------------------------------------------------------
// <copyright file="CustomerBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.10">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Base
{
    using System;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Service.Impl.Customer.Bingo;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasinoSetting.Business;
    using Fanex.LiveCasinoSetting.Model.Log;
    using Fanex.LiveCasinoSetting.Request;

    /// <summary>
    /// Base class for customer controller
    /// </summary>
    public class CustomerBase : BaseController
    {
        private const string CasinoNo = "01";

        private readonly ISettingService setting = new SettingService();
        private readonly IGameSettingService gameSetting = new GameSettingService();
        private readonly ILogWriterService logWriter = new LogWriterService();
        private readonly IReportService report = new ReportService();

        #region Live Casino

        public ActionResult AutoInsertLiveCasinoPost(int custId, int parentId, int uplineRoleId, string groupName)
        {
            var parentInfo = this.setting.GetCustomerInfo(parentId);
            var userInfo = this.setting.GetCustomerInfo(custId);

            if (parentInfo.IsSyncLiveCasino && !userInfo.IsSyncLiveCasino)
            {
                var table = new DataTable();
                var result = this.gameSetting.InsertAccount(this.GetInsertAccountRequest(custId, uplineRoleId, groupName), ref table);

                if (string.IsNullOrWhiteSpace(result.ErrorCode))
                {
                    var custInfo = this.GetCustomerInfo(custId, groupName);

                    this.logWriter.WriteLog(new LogAutoInsertAccount
                    {
                        Action = uplineRoleId == BaseController.Super ? LogActions.AutoInsert_Master : uplineRoleId == BaseController.Master ? LogActions.AutoInsert_Agent : LogActions.AutoInsert_Member,
                        AgentId = custInfo.AgentId,
                        CustId = custId,
                        CustUserName = custInfo.Username,
                        ExchangeId = userInfo.ExChangeId,
                        SignUpIp = this.CurrentContext.Ip4Address,
                        Language = "en",
                        MasterId = custInfo.MasterId,
                        RoleId = custInfo.RoleId,
                        SignUpForm = BaseController.FromSystem,
                        SuperId = custInfo.SuperId,
                        UserId = this.CurrentSession.UserId,
                        UserName = this.CurrentSession.UserName,
                        ErrorCode = result.ErrorCode,
                        ErrorMessage = result.ErrorMessage,
                        dataResult = table
                    });
                }

                return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            }

            return this.JsonBase(new { ErrorCode = 0, ErrorMessage = string.Empty });
        }

        #endregion

        #region Casino

        public ActionResult AutoInsertCasinoPost(int custId, int uplineId, string uplineName)
        {
            try
            {
                var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

                if (custInfo != null && !custInfo.CasinoSync)
                {
                    var isSuccess = false;
                    var customer = new CasinoLibrary.SBAgent.Customer();

                    try
                    {
                        isSuccess = customer.AutoInsertAccount(
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
                            DateTime.Now,
                            CurrentContext.Ip4Address,
                            uplineId,
                            uplineName,
                            BaseController.FromSystem);                            
                    }
                    catch (Exception ex)
                    {
                        Logs.Error(ex.Message, "ExternalAccount", ex);
                    }

                    if (isSuccess)
                    {
                        CasinoManager.Instance.UpdateCSStatus(custId, true, custInfo.CasinoDisabled, this.CurrentSession.UserId, this.CurrentContext.Ip4Address, 0, "En", string.Empty, string.Empty);
                    }
                    else
                    {
                        customer.RemoveAccount(custInfo.UserName, this.CurrentContext.Ip4Address, this.CurrentSession.CustId, this.CurrentSession.UserName, BaseController.FromSystem);
                    }
                }

                return this.JsonBase(new { ErrorCode = 0, ErrorMessage = string.Empty });
            }
            catch (Exception ex)
            {
                Logs.Error(ex.Message, "ExternalAccount", ex);
            }

            return this.JsonBase(new { ErrorCode = 999, ErrorMessage = string.Empty });
        }

        #endregion

        #region Bingo

        public ActionResult AutoInsertBingoPost(int custId, int uplineId, string uplineName)
        {
            try
            {
                var isSuccess = false;
                var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

                if (custInfo != null && !custInfo.BingoSync)
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
                            uplineName,
                            BaseController.FromSystem);
                    }
                    catch (Exception e)
                    {
                        Logs.Error(e.Message, "ExternalAccount", e);
                    }
                }

                if (isSuccess)
                {
                    var err = string.Empty;
                    var result = BingoManager.Instance.UpdateBingoStatus(custId, this.CurrentSession.UserId, 0, true, custInfo.BingoDisabled, true, this.CurrentContext.Ip4Address, "En", ref err);

                    return this.JsonBase(new { ErrorCode = result, ErrorMessage = err });
                }
            }
            catch (Exception ex)
            {
                Logs.Error(ex.Message, "ExternalAccount", ex);
            }
            
            return this.JsonBase(new { ErrorCode = 999, ErrorMessage = string.Empty });
        }

        #endregion

        /// <summary>
        /// Gets customer info
        /// </summary>
        /// <param name="custId">Customer Id</param>
        /// <param name="groupName">Group Name that user belongs to</param>
        /// <returns>CUSTINFO object</returns>
        private CustInfo GetCustomerInfo(int custId, string groupName)
        {
            var custInfo = this.report.GetCustomerInfo(groupName, custId, string.Empty, true);

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

        /// <summary>
        /// Gets insert account request
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineRoleId">Up-line role id</param>
        /// <param name="groupName">Group name</param>
        /// <returns>InsertAccountRequest object</returns>
        private InsertAccountRequest GetInsertAccountRequest(int custId, int uplineRoleId, string groupName)
        {
            var custInfo = this.GetCustomerInfo(custId, groupName);
            var uplineInfo = this.setting.GetCustomerInfo(uplineRoleId == BaseController.Super ? custInfo.SuperId : uplineRoleId == BaseController.Master ? custInfo.MasterId : custInfo.AgentId);

            var result = new InsertAccountRequest()
            {
                AgentId = custInfo.AgentId,
                CustId = custId,
                CustUserName = custInfo.Username,
                ExchangeId = uplineInfo.ExChangeId,
                SignUpIp = this.CurrentContext.Ip4Address,
                IsDisabledLiveCasino = uplineInfo.DisableLiveCasino,
                IsEnableDownline = true,
                Language = "en",
                MasterId = custInfo.MasterId,
                RoleId = custInfo.RoleId,
                SignUpForm = BaseController.FromSystem, // External acc system
                SuperId = custInfo.SuperId,
                SyncLiveCasino = uplineInfo.IsSyncLiveCasino,
                UserId = this.CurrentSession.UserId,
                UserName = this.CurrentSession.UserName,
                IsAutoInsert = true // Auto insert account
            };

            return result;
        }
    }
}

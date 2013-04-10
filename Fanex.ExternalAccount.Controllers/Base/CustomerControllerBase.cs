// --------------------------------------------------------------------------
// <copyright file="CustomerControllerBase.cs" company="Nexcel Solutions Vietnam">
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
    public class CustomerControllerBase : BaseController
    {
        private readonly ISettingService setting = new SettingService();
        private readonly IGameSettingService gameSetting = new GameSettingService();
        private readonly ILogWriterService logWriter = new LogWriterService();
        private readonly IReportService report = new ReportService();

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
                        Action = uplineRoleId == 4 ? LogActions.AutoInsert_Master : uplineRoleId == 3 ? LogActions.AutoInsert_Agent : LogActions.AutoInsert_Member,
                        AgentId = custInfo.AgentId,
                        CustId = custId,
                        CustUserName = custInfo.Username,
                        ExchangeId = userInfo.ExChangeId,
                        SignUpIp = this.CurrentContext.Ip4Address,
                        Language = "en",
                        MasterId = custInfo.MasterId,
                        RoleId = custInfo.RoleId,
                        SignUpForm = 3,
                        SuperId = custInfo.SuperId,
                        UserId = this.CurrentSession.UserId,
                        UserName = this.CurrentSession.UserName,
                        ErrorCode = result.ErrorCode.ToString(),
                        ErrorMessage = result.ErrorMessage,
                        dataResult = table
                    });
                }

                return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            }

            return this.JsonBase(new { ErrorCode = 0, ErrorMessage = string.Empty });
        }

        /// <summary>
        /// Gets insert account request
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="groupName">Group name</param>
        /// <returns>InsertAccountRequest object</returns>
        private InsertAccountRequest GetInsertAccountRequest(int custId, int uplineRoleId, string groupName)
        {
            var custInfo = this.GetCustomerInfo(custId, groupName);
            var uplineInfo = this.setting.GetCustomerInfo(uplineRoleId == 4 ? custInfo.SuperId : uplineRoleId == 3 ? custInfo.MasterId : custInfo.AgentId);

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
                SignUpForm = 3, // External acc system
                SuperId = custInfo.SuperId,
                SyncLiveCasino = uplineInfo.IsSyncLiveCasino,
                UserId = this.CurrentSession.UserId,
                UserName = this.CurrentSession.UserName,
                IsAutoInsert = true // Auto insert account
            };

            return result;
        }

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
    }
}

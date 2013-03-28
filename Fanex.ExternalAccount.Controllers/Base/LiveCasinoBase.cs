// --------------------------------------------------------------------------
// <copyright file="LiveCasinoBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.3.20">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Base
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasinoSetting.Business;
    using Fanex.LiveCasinoSetting.Model;
    using Fanex.LiveCasinoSetting.Model.Log;
    using Fanex.LiveCasinoSetting.Request;
    using Newtonsoft.Json;

    /// <summary>
    /// Base controller for live casino setting
    /// </summary>
    public class LiveCasinoBase : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiveCasinoBase"/> class
        /// </summary>
        public LiveCasinoBase()
        {
            // Inits base properties
            this.GameSetting = new GameSettingService();
            this.PtSetting = new SettingService();
            this.MemberWinLimit = new MemberWinLimitService();
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected ISettingService PtSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected IGameSettingService GameSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected IMemberWinLimitService MemberWinLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Updates member win limit setting
        /// </summary>
        /// <param name="custId">Updated customer id</param>
        /// <param name="isDailyAutoReset">Is daily auto reset</param>
        /// <param name="maxWinLimit">Maximum win limit</param>
        /// <returns>ActionResult object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMemberWinLimit(int custId, bool isDailyAutoReset, decimal maxWinLimit)
        {
            var memberWinLimit = new MemberWinLimit()
            {
                IsDailyAutoReset = isDailyAutoReset,
                MaxWinLimit = maxWinLimit
            };

            var context = new RequestContext()
            {
                CustId = custId,
                Language = "en-US",
                SignUpIp = this.CurrentContext.Ip4Address,
                UserId = this.CurrentSession.UserId
            };

            var result = this.MemberWinLimit.UpdateMemberWinLimit(memberWinLimit, context);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Updates game setting info
        /// </summary>
        /// <param name="uplineRoleId">Role id of up-line</param>
        /// <param name="custId">Customer id</param>
        /// <param name="customer">Customer string object</param>
        /// <returns>JSON string object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateGameSetting(int uplineRoleId, int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var oldGameTypes = this.GameSetting.GetByCustomerId(custId);
            var gameTypes = model.GameSetting;
            var custInfo = this.GetCustomerInfo(custId, model.CustomerInfo.GroupName);

            var req = new InsertAccountRequest
            {
                AgentId = (int)custInfo["recommend"],
                MasterId = (int)custInfo["mrecommend"],
                SuperId = (int)custInfo["srecommend"],
                CustId = custId,
                RoleId = uplineRoleId - 1,
                IsDisabledLiveCasino = model.CustomerInfo.DisableLiveCasino,
                IsEnableDownline = true,
                SyncLiveCasino = model.CustomerInfo.IsSyncLiveCasino,
                SignUpIp = this.CurrentContext.Ip4Address,
                SignUpForm = 3 // External accounting
            };

            gameTypes.ForEach(o =>
            {
                o.BettingLimit.ForEach(s =>
                {
                    s.IsDirty = true;
                });
            });

            var result = this.GameSetting.UpdateGameSetting(uplineRoleId, gameTypes, req);
            var results = new List<object>();

            foreach (var item in result)
            {
                results.Add(new { BettypeId = item.Key, ErrorCode = item.Value.ErrorCode, ErrorMessage = item.Value.ErrorMessage });
            }

            return this.JsonBase(result);
        }

        /// <summary>
        /// Gets customer info
        /// </summary>
        /// <param name="custId">Customer Id</param>
        /// <param name="groupName">Group Name that user belongs to</param>
        /// <returns>DataRow object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        protected DataRow GetCustomerInfo(int custId, string groupName)
        {
            var report = new ReportService();
            var custInfo = report.GetCustomerInfo(groupName, custId, string.Empty, true);
            var item = custInfo.Rows[0];

            return item;
        }

        /// <summary>
        /// Gets log action update stake level
        /// </summary>
        /// <param name="roleId">Role id of customer</param>
        /// <returns>LogActions object</returns>
        protected LogActions LogActionUpdateStakeLevel(int roleId)
        {
            switch (roleId)
            {
                case 3:
                    return LogActions.UpdateStakeLevel_Master;
                case 2:
                    return LogActions.UpdateStakeLevel_Agent;
                case 1:
                    return LogActions.UpdateStakeLevel_Member;
            }

            return LogActions.UpdateStakeLevel_Super;
        }

        /// <summary>
        /// Gets log action manual insert account
        /// </summary>
        /// <param name="roleId">Role id of customer</param>
        /// <returns>LogActions object</returns>
        protected LogActions LogActionManual(int roleId)
        {
            switch (roleId)
            {
                case 1:
                    return LogActions.ManualInsert_Member;
                case 2:
                    return LogActions.ManualInsert_Agent;
                case 3:
                    return LogActions.ManualInsert_Master;
            }

            return LogActions.ManualInsert_Super;
        }
    }
}
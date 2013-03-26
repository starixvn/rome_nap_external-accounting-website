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
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.LiveCasinoSetting.Business;
    using Fanex.LiveCasinoSetting.Model;
    using Fanex.LiveCasinoSetting.Model.Log;
    using Fanex.LiveCasinoSetting.Request;

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
            this.GameResult = new Dictionary<int, Dictionary<int, LiveCasinoSetting.Model.Result>>();
            this.WrongPtIds = new List<int>();
            this.UnsyncIds = new List<int>();
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected List<int> WrongPtIds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected List<int> UnsyncIds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected Dictionary<int, Dictionary<int, LiveCasinoSetting.Model.Result>> GameResult
        {
            get;
            set;
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
        /// Gets or sets a value indicating whether the request is submit button clicked
        /// </summary>
        protected bool IsSubmit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property for current role id of edited user
        /// </summary>
        protected int RequestCustRoleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets property
        /// </summary>
        protected LogActions LogActionUpdateStakeLevel
        {
            get
            {
                switch (this.RequestCustRoleId)
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
        }

        /// <summary>
        /// Gets property
        /// </summary>
        protected LogActions LogActionManual
        {
            get
            {
                switch (this.RequestCustRoleId)
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

        /// <summary>
        /// Updates member win limit setting
        /// </summary>
        /// <param name="custId">Updated customer id</param>
        /// <param name="isDailyAutoReset">Is daily auto reset</param>
        /// <param name="maxWinLimit">Maximum win limit</param>
        /// <returns>ActionResult object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdatMemberWinLimit(int custId, bool isDailyAutoReset, decimal maxWinLimit)
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
    }
}
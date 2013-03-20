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
            this.GameTypes = new List<GameType>();
            this.GameResult = new Dictionary<int, Dictionary<int, Result>>();
            this.WrongPtIds = new List<int>();
            this.UnsyncIds = new List<int>();
            this.Custids = new List<int>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether up-line live casino is disabled or not
        /// </summary>
        protected bool DisableCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether live casino is enabled or not
        /// </summary>
        protected bool EnableLiveCS
        {
            get;
            set;
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
        protected List<int> Custids
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected List<GameType> GameTypes
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
        protected SettingService PtSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected GameSettingService GameSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal PresetPT
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal AutoPresetPT
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal MinPT
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal MaxPT
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal Comm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected decimal NewMemberPT
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected int RequestCustRoleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected int RequestCustId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether live casino is synced
        /// </summary>
        protected bool IsSyncLiveCasino
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether update all member or not
        /// </summary>
        protected bool IsUpdateAllMember
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is auto preset position taking
        /// </summary>
        protected bool IsAutoPresetPT
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
        private LogActions LogActionManual
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
    }
}
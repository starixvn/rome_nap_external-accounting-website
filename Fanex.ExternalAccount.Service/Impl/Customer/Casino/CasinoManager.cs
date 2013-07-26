// --------------------------------------------------------------------------
// <copyright file="CasinoManager.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.11">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.Customer.Casino
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using CasinoLibrary.SBAgent;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer.Casino;
    using Fanex.ExternalAccount.Utilities;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>    
    public class CasinoManager
    {
        private const int Super = 4;
        private const int Master = 3;
        private const int Agent = 2;
        private const int Member = 1;

        private static CasinoManager instance = new CasinoManager();
        private readonly CasinoDao casino = new CasinoDao();

        public static CasinoManager Instance
        {
            get
            {
                return CasinoManager.instance ?? new CasinoManager();
            }
        }

        public static string Lang(string original)
        {
            return original + "_En";
        }

        public List<GameKindDetail> GetGameKinds()
        {
            var gameKinds = new List<GameKindDetail>();
            var basicInfo = new BasicInfo();
            var dt = basicInfo.GetGameKind(null);
            foreach (DataRow row in dt.Rows)
            {
                var gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public bool GetCasinoStatus(int custid)
        {
            return this.casino.GetCasinoStatus(custid);
        }

        public bool UpdateCSStatus(int custid, bool syncCS, bool disabledCS, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            return this.casino.UpdateCSStatus(custid, syncCS, disabledCS, userId, ip, subAccId, lang, username, subUsername);
        }

        public bool UpdateCSStatus(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            string errMsg = string.Empty;
            return this.UpdateCSStatus(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, username, subUsername, out errMsg);
        }

        public bool UpdateCSStatus(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, string username, string subUsername, out string errMsg)
        {
            bool status = this.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
            if (!status)
            {
                return false;
            }

            var cust = new Customer();
            return cust.UpdateCustEnableCasino(custid, !disabledCS, isUpdDownline, ip, custid, subAccId == 0 ? username : subUsername, 2);
        }

        public bool UpdateCSStatusWithoutSync(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang)
        {
            string errMsg = string.Empty;
            return this.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
        }

        public CasinoCustomer GetCustInfoIBC(int custid, int uplineId)
        {            
            return this.casino.GetCustInfoIBC(custid, uplineId);
        }

        public List<GameKindDetail> GetGameKinds(string casinoNo)
        {
            var gameKinds = new List<GameKindDetail>();
            var basicInfo = new BasicInfo();
            var dt = basicInfo.GetGameKind(casinoNo);
            foreach (DataRow row in dt.Rows)
            {
                var gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public List<GameKindDetail> GetGameKinds(int custId)
        {
            var gameKinds = new List<GameKindDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameKind(custId);
            foreach (DataRow row in dt.Rows)
            {
                var gameKind = new GameKindDetail();
                gameKind.CodeID = Utils.ToInt32(row["GameKind"]);
                gameKind.CodeName = Utils.ToString(row[Lang("GameKindName")]);
                gameKind.AgentRollingRate = Utils.ToDouble(row["AgentRollingRate"]);
                gameKind.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameKind.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameKind.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameKind.MAgentRollingRate = Utils.ToDouble(row["MAgentRollingRate"]);
                gameKind.MaxPositionTaking = Utils.ToDouble(row["MaxPositionTaking"]);
                gameKind.MemberRollingRate = Utils.ToDouble(row["MemberRollingRate"]);
                gameKind.MinPositionTaking = Utils.ToDouble(row["MinPositionTaking"]);
                gameKind.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameKind.SMAgentRollingRate = Utils.ToDouble(row["SMAgentRollingRate"]);
                gameKind.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);
                gameKind.SuperSPositionTaking = Utils.ToDouble(row["SuperSPositionTaking"]);

                gameKind.AgentRollingRateUpline = Utils.ToDouble(row["UplineAgentRollingRate"]);
                gameKind.APositionTakingUpline = Utils.ToDouble(row["UplineAPositionTaking"]);
                gameKind.AutoPositionTakingUpline = Utils.ToDouble(row["UplineAutoPositionTaking"]);
                gameKind.IsAutoPositionTakingUpline = Utils.ToBoolean(row["UplineIsAutoPositionTaking"]);
                gameKind.MAgentRollingRateUpline = Utils.ToDouble(row["UplineMAgentRollingRate"]);
                gameKind.MaxPositionTakingUpline = Utils.ToDouble(row["UplineMaxPositionTaking"]);
                gameKind.MemberRollingRateUpline = Utils.ToDouble(row["UplineMemberRollingRate"]);
                gameKind.MinPositionTakingUpline = Utils.ToDouble(row["UplineMinPositionTaking"]);
                gameKind.MPositionTakingUpline = Utils.ToDouble(row["UplineMPositionTaking"]);
                gameKind.SMAgentRollingRateUpline = Utils.ToDouble(row["UplineSMAgentRollingRate"]);
                gameKind.SPositionTakingUpline = Utils.ToDouble(row["UplineSPositionTaking"]);

                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public List<GameKindDetail> GetGameKinds_EditMember(int memberId)
        {
            var gameKinds = new List<GameKindDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameKind_EditMember(memberId);
            foreach (DataRow row in dt.Rows)
            {
                var gameKind = new GameKindDetail();
                gameKind.CodeID = Utils.ToInt32(row["GameKind"]);
                gameKind.CodeName = Utils.ToString(row[Lang("GameKindName")]);
                gameKind.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameKind.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameKind.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameKind.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameKind.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);
                gameKind.SuperMaxPositionTaking = Utils.ToDouble(row["SuperMaxPositionTaking"]);
                gameKind.SuperMinPositionTaking = Utils.ToDouble(row["SuperMinPositionTaking"]);

                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public List<GameTypeDetail> GetGameTypes_EditMember(int memberId, int gameKindID)
        {
            var gameTypes = new List<GameTypeDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameType_EditMember(memberId, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                var gameType = new GameTypeDetail();

                gameType.GameTypeID = Utils.ToInt32(row["GameTypeID"]);
                gameType.GameTypeName = Utils.ToString(row[Lang("GameTypeName")]);
                gameType.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameType.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameType.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameType.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameType.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);
                gameType.SuperMaxPositionTaking = Utils.ToDouble(row["SuperMaxPositionTaking"]);
                gameType.SuperMinPositionTaking = Utils.ToDouble(row["SuperMinPositionTaking"]);

                gameTypes.Add(gameType);
            }

            return gameTypes;
        }

        public List<GameTypeDetail> GetGameType(int gameKindID)
        {
            var gameTypes = new List<GameTypeDetail>();
            var basicInfo = new BasicInfo();
            var dt = basicInfo.GetGameType(gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                var gameType = new GameTypeDetail();
                gameType.CasinoID = Utils.ToInt32(row["CasinoID"]);
                gameType.GameTypeID = Utils.ToInt32(row["GameTypeID"]);
                gameType.GameTypeName = Utils.ToString(row[Lang("GameTypeName")]);
                gameType.GameTypeNo = Utils.ToString(row["GameTypeNo"]);
                gameType.GameKind = Utils.ToInt32(row["GameKind"]);
                gameType.GameGroupID = Utils.ToInt32(row["GameGroupID"]);
                gameType.FlashFileName = Utils.ToString(row["FlashFileName"]);
                gameType.AlertMaxIdle = Utils.ToInt32(row["AlertMaxIdle"]);
                gameType.IsTrialTime = Utils.ToBoolean(row["IsTrialTime"]);
                gameType.MaxIdle = Utils.ToInt32(row["MaxIdle"]);
                gameTypes.Add(gameType);
            }

            return gameTypes;
        }

        public List<GameTypeDetail> GetGameType(int custId, int gameKindID)
        {
            var gameTypes = new List<GameTypeDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameType(custId, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                var gameType = new GameTypeDetail();

                gameType.GameTypeID = Utils.ToInt32(row["GameTypeID"]);
                gameType.GameTypeName = Utils.ToString(row[Lang("GameTypeName")]);
                gameType.AgentRollingRate = Utils.ToDouble(row["AgentRollingRate"]);
                gameType.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameType.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameType.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameType.MAgentRollingRate = Utils.ToDouble(row["MAgentRollingRate"]);
                gameType.MaxPositionTaking = Utils.ToDouble(row["MaxPositionTaking"]);
                gameType.MemberRollingRate = Utils.ToDouble(row["MemberRollingRate"]);
                gameType.MinPositionTaking = Utils.ToDouble(row["MinPositionTaking"]);
                gameType.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameType.SMAgentRollingRate = Utils.ToDouble(row["SMAgentRollingRate"]);
                gameType.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);

                gameType.AgentRollingRateUpline = Utils.ToDouble(row["UplineAgentRollingRate"]);
                gameType.APositionTakingUpline = Utils.ToDouble(row["UplineAPositionTaking"]);
                gameType.AutoPositionTakingUpline = Utils.ToDouble(row["UplineAutoPositionTaking"]);
                gameType.IsAutoPositionTakingUpline = Utils.ToBoolean(row["UplineIsAutoPositionTaking"]);
                gameType.MAgentRollingRateUpline = Utils.ToDouble(row["UplineMAgentRollingRate"]);
                gameType.MaxPositionTakingUpline = Utils.ToDouble(row["UplineMaxPositionTaking"]);
                gameType.MemberRollingRateUpline = Utils.ToDouble(row["UplineMemberRollingRate"]);
                gameType.MinPositionTakingUpline = Utils.ToDouble(row["UplineMinPositionTaking"]);
                gameType.MPositionTakingUpline = Utils.ToDouble(row["UplineMPositionTaking"]);
                gameType.SMAgentRollingRateUpline = Utils.ToDouble(row["UplineSMAgentRollingRate"]);
                gameType.SPositionTakingUpline = Utils.ToDouble(row["UplineSPositionTaking"]);

                gameTypes.Add(gameType);
            }

            return gameTypes;
        }

        public List<GameKindDetail> GetGameKinds_MasterEditMember(int memberid)
        {
            var gameKinds = new List<GameKindDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameKind(memberid);
            foreach (DataRow row in dt.Rows)
            {
                var gameKind = new GameKindDetail();
                gameKind.CodeID = Utils.ToInt32(row["GameKind"]);
                gameKind.CodeName = Utils.ToString(row[Lang("GameKindName")]);
                gameKind.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameKind.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameKind.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameKind.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameKind.MasterMaxPositionTaking = Utils.ToDouble(row["MasterMaxPositionTalking"]);
                gameKind.MasterMinPositionTaking = Utils.ToDouble(row["MasterMinPositionTaking"]);
                gameKind.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);
                gameKind.SuperSPositionTaking = Utils.ToDouble(row["SuperSPositionTaking"]);

                gameKind.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameKind.IsAutoPositionTakingUpline = Utils.ToBoolean(row["SuperIsAutoPositionTaking"]);

                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public List<GameTypeDetail> GetGameTypes_MasterEditMember(int memberid, int gameKindID)
        {
            var gameTypes = new List<GameTypeDetail>();
            var cust = new Customer();
            var dt = cust.GetCustGameType(memberid, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                var gameType = new GameTypeDetail();

                gameType.GameTypeID = Utils.ToInt32(row["GameTypeID"]);
                gameType.GameTypeName = Utils.ToString(row[Lang("GameTypeName")]);
                gameType.AgentRollingRate = Utils.ToDouble(row["AgentRollingRate"]);
                gameType.APositionTaking = Utils.ToDouble(row["APositionTaking"]);
                gameType.AutoPositionTaking = Utils.ToDouble(row["AutoPositionTaking"]);
                gameType.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameType.MPositionTaking = Utils.ToDouble(row["MPositionTaking"]);
                gameType.MasterMaxPositionTaking = Utils.ToDouble(row["MasterMaxPositionTalking"]);
                gameType.MasterMinPositionTaking = Utils.ToDouble(row["MasterMinPositionTaking"]);
                gameType.SPositionTaking = Utils.ToDouble(row["SPositionTaking"]);
                gameType.SuperSPositionTaking = Utils.ToDouble(row["SuperSPositionTaking"]);
                gameType.IsAutoPositionTaking = Utils.ToBoolean(row["IsAutoPositionTaking"]);
                gameType.IsAutoPositionTakingUpline = Utils.ToBoolean(row["SuperIsAutoPositionTaking"]);
                gameTypes.Add(gameType);
            }

            return gameTypes;
        }

        public List<StakeLevelDetail> GetCustStakeLevels(int custId, int gameTypeID)
        {
            var stakeLevels = new List<StakeLevelDetail>();
            var cust = new Customer();
            var dt = cust.GetCustStakeLevel(custId, gameTypeID);
            foreach (DataRow row in dt.Rows)
            {
                var stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = Utils.ToBoolean(row["IsChecked"]);
                stakeLevels.Add(stake);
            }

            return stakeLevels;
        }

        public List<StakeLevelDetail> GetGameTypeStakeLevels(int gameTypeID, int currencyId)
        {
            var stakeLevels = new List<StakeLevelDetail>();
            var bi = new BasicInfo();
            var dt = bi.GetGameTypeStakeLevel(gameTypeID, currencyId);
            foreach (DataRow row in dt.Rows)
            {
                var stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = true;
                stakeLevels.Add(stake);
            }

            return stakeLevels;
        }

        public CasinoCustomer GetCustSettingCS(int custId)
        {
            var cust = new Customer();
            var dt = cust.GetCustSetting(custId);
            DataRow row = null;
            var custSetting = new CasinoCustomer();
            if (dt.Rows.Count > 0)
            {
                row = dt.Rows[0];
                custSetting.IsEnablePT = Utils.ToBoolean(row["IsEnablePT"]);
                custSetting.TotalWinLoss = Utils.ToDouble(row["ActualMaxTransfer"]);
                custSetting.MaxWin = Utils.ToDouble(row["MaxWin"]);
                custSetting.MaxLose = Utils.ToDouble(row["MaxLose"]);
                custSetting.MaxTransferCredit = Utils.ToDouble(row["MaxTransferCredit"]);
                custSetting.IsMaxWin = Utils.ToBoolean(row["IsMaxWin"]);
                custSetting.IsMaxLose = Utils.ToBoolean(row["IsMaxLose"]);
                custSetting.PTByGameType = Utils.ToBoolean(row["PTByGameType"]);
                custSetting.RoleId = Utils.ToInt32(row["RoleId"]);
                custSetting.UpperCustID = Utils.ToInt32(row["UpperCustId"]);
                custSetting.IsAutoReset = Utils.ToBoolean(row["IsAutoReset"]);

                // Get max transfer information
                if (custSetting.UpperCustID != 0)
                {
                    var dt2 = cust.GetDefaultMaxTransfer(custSetting.UpperCustID, 0, false);
                    custSetting.MaxTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MaxLimit"]);
                    custSetting.MinTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MinLimit"]);

                    custSetting.DefaultTransferCredit = Utils.ToDouble(dt2.Rows[0]["DefMaxTransferCredit"]);
                }

                custSetting.NoCSData = false;
            }

            return custSetting;
        }

        public void GetCustSettingCS(ref CasinoCustomer custIBC)
        {
            var cust = new Customer();
            var dt = cust.GetCustSetting(custIBC.IBCCustID);
            DataRow row = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                row = dt.Rows[0];
                custIBC.IsEnablePT = Utils.ToBoolean(row["IsEnablePT"]);
                custIBC.TotalWinLoss = Utils.ToDouble(row["ActualMaxTransfer"]);
                custIBC.MaxWin = Utils.ToDouble(row["MaxWin"]);
                custIBC.MaxLose = Utils.ToDouble(row["MaxLose"]);

                custIBC.MaxTransferCredit = Utils.ToDouble(row["MaxTransferCredit"]);

                custIBC.IsMaxWin = Utils.ToBoolean(row["IsMaxWin"]);
                custIBC.IsMaxLose = Utils.ToBoolean(row["IsMaxLose"]);
                custIBC.PTByGameType = Utils.ToBoolean(row["PTByGameType"]);
                custIBC.PTByGameKind = Utils.ToBoolean(row["PTByGameKind"]);
                custIBC.RoleId = Utils.ToInt32(row["RoleId"]);
                custIBC.UpperCustID = Utils.ToInt32(row["UpperCustId"]);
                custIBC.IsAutoReset = Utils.ToBoolean(row["IsAutoReset"]);

                // Get max transfer information
                if (custIBC.UpperCustID != 0)
                {
                    var dt2 = cust.GetDefaultMaxTransfer(custIBC.UpperCustID, 0, false);
                    custIBC.MaxTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MaxLimit"]);
                    custIBC.MinTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MinLimit"]);

                    custIBC.DefaultTransferCredit = Utils.ToDouble(dt2.Rows[0]["DefMaxTransferCredit"]);
                }

                custIBC.NoCSData = false;
            }
        }

        public Result Update(int custId, int roleId, int uplineId, int uplineRoleId, string ip, int modifiedId, string modifiedBy, int fromSystem, CasinoObject casino)
        {
            var errMsg = string.Empty;
            var customer = new Customer();
            var pts = casino.Pts;
            var casinoGame = casino.CasinoGame;
            var miniGame = casino.MiniGame;
            var casinoInfo = casino.CasinoInfo;
            var customerInfo = casino.CustomerInfo;
            var messages = string.Empty;
            var settingsInserted = true;
            var statusUpdated = true;
            var maxTransferUpdated = false;
            var casinoPTResult = new Result();
            var enablePTUpdated = false;

            // Check whether casino is synced or not to insert customer settings for direct downline only
            if (!customerInfo.CasinoSync && uplineRoleId - roleId == 1)
            {
                settingsInserted = customer.InsertCustSetting(
                    "01", 
                    customerInfo.IBCCustID, 
                    customerInfo.UserName, 
                    customerInfo.UserName2, 
                    customerInfo.FirstName, 
                    customerInfo.LastName, 
                    customerInfo.UserPwd, 
                    customerInfo.Phone, 
                    customerInfo.MobilePhone, 
                    customerInfo.Currency, 
                    customerInfo.Fax, 
                    customerInfo.Closed, 
                    customerInfo.Suspend, 
                    customerInfo.IsEnablePT, 
                    (decimal)customerInfo.MaxWin, 
                    (decimal)customerInfo.MaxLose, 
                    true, 
                    (decimal)customerInfo.MaxTransferCredit, 
                    customerInfo.SIBCCustID, 
                    customerInfo.MIBCCustID, 
                    customerInfo.AIBCCustID, 
                    customerInfo.PTByGameType, 
                    roleId.ToString(), 
                    DateTime.Now, 
                    ip, 
                    modifiedId, 
                    modifiedBy, 
                    fromSystem, 
                    customerInfo.IsAutoReset,
                    false,
                    false);
            }

            // Update Status for direct downline only
            if (((settingsInserted && !customerInfo.CasinoSync) || customerInfo.CasinoSync) && uplineRoleId - roleId == 1)
            {                
                statusUpdated = this.UpdateCSStatus(
                    custId, 
                    true, 
                    customerInfo.CasinoDisabled, 
                    customerInfo.EnableDownline, 
                    uplineId, 
                    string.Empty, 
                    0, 
                    "en", 
                    customerInfo.UserName, 
                    string.Empty, 
                    out errMsg);
            }

            // Enable PT/Update max transfer for direct downline only
            if (statusUpdated && uplineRoleId - roleId == 1)
            {
                enablePTUpdated = customer.UpdateCustEnablePT(custId, true);

                maxTransferUpdated = customer.UpdateMaxTransfer(
                    custId, 
                    true, 
                    (decimal)casinoInfo.MaxTransferCredit, 
                    casinoInfo.IsAutoReset, 
                    ip, 
                    uplineId, 
                    modifiedBy, 
                    fromSystem);
            }

            // Update PT/Betting limit settings
            casinoPTResult = this.UpdateSettings(
                custId, 
                roleId, 
                uplineId, 
                uplineRoleId, 
                ip, 
                modifiedId, 
                modifiedBy, 
                fromSystem, 
                casino);

            // Gets error messages
            messages += string.IsNullOrWhiteSpace(errMsg) && casinoPTResult.IsSuccess ? string.Empty : !string.IsNullOrWhiteSpace(errMsg) ? errMsg + "<br />" + casinoPTResult.ErrorMessage : casinoPTResult.ErrorMessage;

            var msgs = messages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var retMsg = string.Empty;

            foreach (var item in msgs)
            {
                retMsg += string.Format("{0}{1}", item, ", ");
            }

            return new Result(retMsg == string.Empty || retMsg == "<br />" ? 0 : 1, retMsg);
        }

        private bool DoUpdateCSStatusWithoutSync(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, out string errMsg)
        {
            return this.casino.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
        }

        private Result UpdateSettings(int custId, int roleId, int uplineId, int uplineRoleId, string ip, int modifiedId, string modifiedBy, int fromSystem, CasinoObject casino)
        {
            var customer = new Customer();
            var msg = string.Empty;
            var gameKindTable = (uplineRoleId == Super || uplineRoleId == Master) && roleId == Member ? 
                this.GameKindsDataTableMember() : this.GameKindsDataTable();
            var gameTypeTable = (uplineRoleId == Super || uplineRoleId == Master) && roleId == Member ? 
                this.GameTypesDataTableMember() : this.GameTypesDataTable();
            var gameStakeTable = this.GameTypesStakesDataTable();

            var gameKinds = this.GetGameKinds(custId);
                                    
            if (gameKinds != null && gameKinds.Count == 2)
            {
                var casinoPT = casino.Pts[0];
                var miniPT = casino.Pts[1];
                var gameKindCasinoID = gameKinds[0].CodeID;
                var gameKindMiniID = gameKinds[1].CodeID;

                if (uplineRoleId - roleId != 1)
                {
                    // Assign data for indirect downline
                    gameKindTable.Rows.Add(
                        new object[] 
                            {
                                custId,
                                gameKindCasinoID,
                                !(uplineRoleId == Super || uplineRoleId == Master) || casinoPT.IsAutoPositionTaking ? null : (object)casinoPT.SPositionTaking,
                                !(uplineRoleId == Master || uplineRoleId == Agent) ? null : (object)casinoPT.MPositionTaking,
                                casinoPT.IsAutoPositionTaking ? 1 : 0,
                                !casinoPT.IsAutoPositionTaking ? null : (object)casinoPT.AutoPositionTaking,
                                uplineRoleId,
                                ip,
                                modifiedId, 
                                modifiedBy, 
                                fromSystem
                            });

                    gameKindTable.Rows.Add(
                        new object[] 
                            {
                                custId,
                                gameKindMiniID,
                                !(uplineRoleId == Super || uplineRoleId == Master) || miniPT.IsAutoPositionTaking ? null : (object)miniPT.SPositionTaking,
                                !(uplineRoleId == Master || uplineRoleId == Agent) ? null : (object)miniPT.MPositionTaking,
                                miniPT.IsAutoPositionTaking ? 1 : 0,
                                !miniPT.IsAutoPositionTaking ? null : (object)miniPT.AutoPositionTaking,
                                uplineRoleId,
                                ip,
                                modifiedId, 
                                modifiedBy, 
                                fromSystem
                            });
                }
                else
                {
                    // Assign data for direct downline
                    gameKindTable.Rows.Add(
                        new object[]
                            {
                                custId,
                                gameKindCasinoID,
                                null,
                                !(roleId == Super || roleId == Master) || casinoPT.IsAutoPositionTaking ? null : (object)casinoPT.SPositionTaking,
                                !(roleId == Master || roleId == Agent) ? null : (object)casinoPT.MPositionTaking,
                                !(roleId == Agent || roleId == Member) ? null : (object)casinoPT.APositionTaking,
                                casinoPT.IsAutoPositionTaking ? 1 : 0,
                                !casinoPT.IsAutoPositionTaking ? null : (object)casinoPT.AutoPositionTaking,
                                !(roleId == Super || roleId == Master || roleId == Agent) ? null : (object)casinoPT.MinPositionTaking,
                                0,
                                roleId != Super ? null : (object)casinoPT.SMAgentRollingRate,
                                roleId != Master ? null : (object)casinoPT.MAgentRollingRate,
                                roleId != Agent ? null : (object)casinoPT.AgentRollingRate,
                                roleId != Member ? null : (object)casinoPT.MemberRollingRate,
                                roleId,
                                ip,
                                modifiedId, 
                                modifiedBy, 
                                fromSystem
                            });

                    gameKindTable.Rows.Add(
                        new object[]
                            {
                                custId,
                                gameKindMiniID,
                                null,
                                !(roleId == Super || roleId == Master) || miniPT.IsAutoPositionTaking ? null : (object)miniPT.SPositionTaking,
                                !(roleId == Master || roleId == Agent) ? null : (object)miniPT.MPositionTaking,
                                !(roleId == Agent || roleId == Member) ? null : (object)miniPT.APositionTaking,
                                miniPT.IsAutoPositionTaking ? 1 : 0,
                                !miniPT.IsAutoPositionTaking ? null : (object)miniPT.AutoPositionTaking,
                                !(roleId == Super || roleId == Master || roleId == Agent) ? null : (object)miniPT.MinPositionTaking,
                                0,
                                roleId != Super ? null : (object)miniPT.SMAgentRollingRate,
                                roleId != Master ? null : (object)miniPT.MAgentRollingRate,
                                roleId != Agent ? null : (object)miniPT.AgentRollingRate,
                                roleId != Member ? null : (object)miniPT.MemberRollingRate,
                                roleId,
                                ip,
                                modifiedId, 
                                modifiedBy, 
                                fromSystem
                            });
                }
                
                foreach (var item in casino.CasinoGame)
                {
                    if (uplineRoleId - roleId != 1)
                    {
                        gameTypeTable.Rows.Add(
                            new object[] 
                        {
                            custId,
                            item.GameTypeID,
                            !(uplineRoleId == Super || uplineRoleId == Master) || item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.SPositionTaking,
                            !(uplineRoleId == Master || uplineRoleId == Agent) ? null : (object)item.Pt.MPositionTaking,
                            item.Pt.IsAutoPositionTaking ? 1 : 0,
                            !item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.AutoPositionTaking,
                            roleId,
                            ip,
                            modifiedId, 
                            modifiedBy, 
                            fromSystem
                        });
                    }
                    else
                    {
                        gameTypeTable.Rows.Add(
                            new object[]
                        {
                            custId,
                            item.GameTypeID,
                            null,
                            !(roleId == Super || roleId == Master) || item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.SPositionTaking,
                            !(roleId == Master || roleId == Agent) ? null : (object)item.Pt.MPositionTaking,
                            !(roleId == Agent || roleId == Member) ? null : (object)item.Pt.APositionTaking,
                            item.Pt.IsAutoPositionTaking ? 1 : 0,
                            !item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.AutoPositionTaking,
                            !(roleId == Super || roleId == Master || roleId == Member) ? null : (object)item.Pt.MinPositionTaking,
                            0,
                            roleId != Super ? null : (object)item.Pt.SMAgentRollingRate,
                            roleId != Master ? null : (object)item.Pt.MAgentRollingRate,
                            roleId != Agent ? null : (object)item.Pt.AgentRollingRate,
                            roleId != Member ? null : (object)item.Pt.MemberRollingRate,
                            this.SelectedStakeLevels(item.StakeLevels),
                            roleId,
                            ip,
                            modifiedId, 
                            modifiedBy, 
                            fromSystem
                        });
                    }

                    gameStakeTable.Rows.Add(
                        new object[] 
                    {
                        custId,
                        item.GameTypeID,
                        this.SelectedStakeLevels(item.StakeLevels),
                        roleId,
                        ip,
                        modifiedId, 
                        modifiedBy, 
                        fromSystem
                    });
                }

                foreach (var item in casino.MiniGame)
                {
                    if (uplineRoleId - roleId != 1)
                    {
                        gameTypeTable.Rows.Add(
                            new object[] 
                        {
                            custId,
                            item.GameTypeID,
                            !(uplineRoleId == Super || uplineRoleId == Master) || item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.SPositionTaking,
                            !(uplineRoleId == Master || uplineRoleId == Agent) ? null : (object)item.Pt.MPositionTaking,
                            item.Pt.IsAutoPositionTaking ? 1 : 0,
                            !item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.AutoPositionTaking,
                            roleId,
                            ip,
                            modifiedId, 
                            modifiedBy, 
                            fromSystem
                        });
                    }
                    else
                    {
                        gameTypeTable.Rows.Add(
                            new object[]
                        {
                            custId,
                            item.GameTypeID,
                            null,
                            !(roleId == Super || roleId == Master) || item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.SPositionTaking,
                            !(roleId == Master || roleId == Agent) ? null : (object)item.Pt.MPositionTaking,
                            !(roleId == Agent || roleId == Member) ? null : (object)item.Pt.APositionTaking,
                            item.Pt.IsAutoPositionTaking ? 1 : 0,
                            !item.Pt.IsAutoPositionTaking ? null : (object)item.Pt.AutoPositionTaking,
                            !(roleId == Super || roleId == Master || roleId == Member) ? null : (object)item.Pt.MinPositionTaking,
                            0,
                            roleId != Super ? null : (object)item.Pt.SMAgentRollingRate,
                            roleId != Master ? null : (object)item.Pt.MAgentRollingRate,
                            roleId != Agent ? null : (object)item.Pt.AgentRollingRate,
                            roleId != Member ? null : (object)item.Pt.MemberRollingRate,
                            this.SelectedStakeLevels(item.StakeLevels),
                            roleId,
                            ip,
                            modifiedId, 
                            modifiedBy, 
                            fromSystem
                        });
                    }

                    gameStakeTable.Rows.Add(
                        new object[] 
                    {
                        custId,
                        item.GameTypeID,
                        this.SelectedStakeLevels(item.StakeLevels),
                        roleId,
                        ip,
                        modifiedId, 
                        modifiedBy, 
                        fromSystem
                    });
                }

                var resultKind = new DataTable();
                var resultType = new DataTable();
                var resultStake = new DataTable();

                if (uplineRoleId - roleId == 1)
                {
                    // Update direct downline
                    resultKind = customer.UpdateCustGameKind(gameKindTable);
                    
                    if (casino.CasinoInfo.PTByGameType)
                    {
                        resultType = customer.UpdateCustGameType(gameTypeTable);
                    }
                }
                else
                {
                    // Update indirect downline
                    resultKind = customer.UpdateCustGameKind_EditMember(gameKindTable);
                    
                    if (casino.CasinoInfo.PTByGameType)
                    {
                        resultType = customer.UpdateCustGameType_EditMember(gameTypeTable);
                    }
                }

                resultStake = customer.UpdateCustGameTypeStakeLevel(gameStakeTable);

                // Process error messages
                foreach (DataRow item in resultKind.Rows)
                {
                    msg += item.Field<string>("ErrMsg") + "<br />";
                }

                if (casino.CasinoInfo.PTByGameType)
                {
                    foreach (DataRow item in resultType.Rows)
                    {
                        msg += item.Field<string>("ErrMsg") + "<br />";
                    }
                }

                foreach (DataRow item in resultStake.Rows)
                {
                    msg += item.Field<string>("ErrMsg") + "<br />";
                }
                
                msg = msg.EndsWith("<br />") ? msg.Substring(0, msg.IndexOf("<br />")) : msg;

                return new Result(msg == string.Empty || (resultKind.Rows.Count == 0 && resultType.Rows.Count == 0 && resultStake.Rows.Count == 0) ? 0 : 1, msg);
            }
            
            return new Result(1, "Invalid data input");
        }
        
        private string SelectedStakeLevels(List<StakeLevelDetail> stakes)
        {
            var selectedLevels = string.Empty;

            stakes.ForEach(item =>
            {
                selectedLevels += item.IsChecked ? item.StakeLevelID + "," : string.Empty;
            });

            return selectedLevels.EndsWith(",") ? selectedLevels.Substring(0, selectedLevels.Length - 1) : selectedLevels;
        }

        private DataTable GameKindsDataTableMember()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[] 
                {
                    new DataColumn("IBCCustID"),
                    new DataColumn("GameKind"),
                    new DataColumn("SPositionTaking"),
                    new DataColumn("MPositionTaking"),
                    new DataColumn("IsAutoPositionTaking"),
                    new DataColumn("AutoPositionTaking"),
                    new DataColumn("RoleId"),
                    new DataColumn("FromIP"),
                    new DataColumn("ModifyBy"), 
                    new DataColumn("ModifyUserName"), 
                    new DataColumn("FromSystem")
                });

            return dt;
        }

        private DataTable GameTypesDataTableMember()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[] 
                {
                    new DataColumn("IBCCustID"),
                    new DataColumn("GameTypeID"),
                    new DataColumn("SPositionTaking"),
                    new DataColumn("MPositionTaking"),
                    new DataColumn("IsAutoPositionTaking"),
                    new DataColumn("AutoPositionTaking"),
                    new DataColumn("RoleId"),
                    new DataColumn("FromIP"),
                    new DataColumn("ModifyBy"), 
                    new DataColumn("ModifyUserName"), 
                    new DataColumn("FromSystem")
                });

            return dt;
        }

        private DataTable GameKindsDataTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[] 
                {
                    new DataColumn("IBCCustID"),
                    new DataColumn("GameKind"),
                    new DataColumn("HPositionTaking"),
                    new DataColumn("SPositionTaking"),
                    new DataColumn("MPositionTaking"),
                    new DataColumn("APositionTaking"),
                    new DataColumn("IsAutoPositionTaking"),
                    new DataColumn("AutoPositionTaking"),
                    new DataColumn("MinPositionTaking"),
                    new DataColumn("MaxPositionTaking"),
                    new DataColumn("SMAgentRollingRate"),
                    new DataColumn("MAgentRollingRate"),
                    new DataColumn("AgentRollingRate"),
                    new DataColumn("MemberRollingRate"),
                    new DataColumn("RoleId"),
                    new DataColumn("FromIP"),
                    new DataColumn("ModifyBy"), 
                    new DataColumn("ModifyUserName"), 
                    new DataColumn("FromSystem")
                });

            return dt;
        }

        private DataTable GameTypesDataTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[]
                {
                    new DataColumn("IBCCustID"),
                    new DataColumn("GameTypeID"),
                    new DataColumn("HPositionTaking"),
                    new DataColumn("SPositionTaking"),
                    new DataColumn("MPositionTaking"),
                    new DataColumn("APositionTaking"),
                    new DataColumn("IsAutoPositionTaking"),
                    new DataColumn("AutoPositionTaking"),
                    new DataColumn("MinPositionTaking"),
                    new DataColumn("MaxPositionTaking"),
                    new DataColumn("SMAgentRollingRate"),
                    new DataColumn("MAgentRollingRate"),
                    new DataColumn("AgentRollingRate"),
                    new DataColumn("MemberRollingRate"),
                    new DataColumn("StakeLevelList"),
                    new DataColumn("RoleId"),
                    new DataColumn("FromIP"),
                    new DataColumn("ModifyBy"), 
                    new DataColumn("ModifyUserName"), 
                    new DataColumn("FromSystem")
                });

            return dt;
        }

        private DataTable GameTypesStakesDataTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(
                new DataColumn[] 
                {
                    new DataColumn("IBCCustID"),
                    new DataColumn("GameTypeID"),
                    new DataColumn("StakeLevelList"),
                    new DataColumn("RoleId"),
                    new DataColumn("FromIP"),
                    new DataColumn("ModifyBy"), 
                    new DataColumn("ModifyUserName"), 
                    new DataColumn("FromSystem")
                });

            return dt;
        }
    }
}

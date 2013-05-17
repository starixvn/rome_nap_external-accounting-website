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
    using System.Collections.Generic;
    using System.Data;
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
            List<GameKindDetail> gameKinds = new List<GameKindDetail>();
            BasicInfo basicInfo = new BasicInfo();
            DataTable dt = basicInfo.GetGameKind(null);
            foreach (DataRow row in dt.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
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

            Customer cust = new Customer();
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
            List<GameKindDetail> gameKinds = new List<GameKindDetail>();
            BasicInfo basicInfo = new BasicInfo();
            DataTable dt = basicInfo.GetGameKind(casinoNo);
            foreach (DataRow row in dt.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
                gameKinds.Add(gameKind);
            }

            return gameKinds;
        }

        public List<GameKindDetail> GetGameKinds(int custId)
        {
            List<GameKindDetail> gameKinds = new List<GameKindDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameKind(custId);
            foreach (DataRow row in dt.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail();
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
            List<GameKindDetail> gameKinds = new List<GameKindDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameKind_EditMember(memberId);
            foreach (DataRow row in dt.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail();
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
            List<GameTypeDetail> gameTypes = new List<GameTypeDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameType_EditMember(memberId, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                GameTypeDetail gameType = new GameTypeDetail();

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
            List<GameTypeDetail> gameTypes = new List<GameTypeDetail>();
            BasicInfo basicInfo = new BasicInfo();
            DataTable dt = basicInfo.GetGameType(gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                GameTypeDetail gameType = new GameTypeDetail();
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
            List<GameTypeDetail> gameTypes = new List<GameTypeDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameType(custId, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                GameTypeDetail gameType = new GameTypeDetail();

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
            List<GameKindDetail> gameKinds = new List<GameKindDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameKind(memberid);
            foreach (DataRow row in dt.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail();
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
            List<GameTypeDetail> gameTypes = new List<GameTypeDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustGameType(memberid, gameKindID);
            foreach (DataRow row in dt.Rows)
            {
                GameTypeDetail gameType = new GameTypeDetail();

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
            List<StakeLevelDetail> stakeLevels = new List<StakeLevelDetail>();
            Customer cust = new Customer();
            DataTable dt = cust.GetCustStakeLevel(custId, gameTypeID);
            foreach (DataRow row in dt.Rows)
            {
                StakeLevelDetail stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = Utils.ToBoolean(row["IsChecked"]);
                stakeLevels.Add(stake);
            }

            return stakeLevels;
        }

        public List<StakeLevelDetail> GetGameTypeStakeLevels(int gameTypeID, int currencyId)
        {
            List<StakeLevelDetail> stakeLevels = new List<StakeLevelDetail>();
            BasicInfo bi = new BasicInfo();
            DataTable dt = bi.GetGameTypeStakeLevel(gameTypeID, currencyId);
            foreach (DataRow row in dt.Rows)
            {
                StakeLevelDetail stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = true;
                stakeLevels.Add(stake);
            }

            return stakeLevels;
        }

        public CasinoCustomer GetCustSettingCS(int custId)
        {
            Customer cust = new Customer();
            DataTable dt = cust.GetCustSetting(custId);
            DataRow row = null;
            CasinoCustomer custSetting = new CasinoCustomer();
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
                    DataTable dt2 = cust.GetDefaultMaxTransfer(custSetting.UpperCustID, 0, false);
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
            Customer cust = new Customer();
            DataTable dt = cust.GetCustSetting(custIBC.IBCCustID);
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
                custIBC.RoleId = Utils.ToInt32(row["RoleId"]);
                custIBC.UpperCustID = Utils.ToInt32(row["UpperCustId"]);
                custIBC.IsAutoReset = Utils.ToBoolean(row["IsAutoReset"]);

                // Get max transfer information
                if (custIBC.UpperCustID != 0)
                {
                    DataTable dt2 = cust.GetDefaultMaxTransfer(custIBC.UpperCustID, 0, false);
                    custIBC.MaxTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MaxLimit"]);
                    custIBC.MinTransferCreditLimit = Utils.ToDouble(dt2.Rows[0]["MinLimit"]);

                    custIBC.DefaultTransferCredit = Utils.ToDouble(dt2.Rows[0]["DefMaxTransferCredit"]);
                }

                custIBC.NoCSData = false;
            }
        }

        public Result Update(int custId, int roleId, int uplineId, int uplineRoleId, string ip, string modifiedBy, int fromSystem, CasinoObject casino)
        {
            var errMsg = string.Empty;
            var customer = new Customer();
            var pts = casino.Pts;
            var casinoGame = casino.CasinoGame;
            var miniGame = casino.MiniGame;
            var casinoInfo = casino.CasinoInfo;
            var message = string.Empty;

            // Update Status
            var status = this.UpdateCSStatus(custId, true, false, true, uplineId, string.Empty, 0, "en", casinoInfo.UserName, string.Empty, out errMsg);

            // Update Max Transfer
            var maxTransfer = customer.UpdateMaxTransfer(custId, true, (decimal)casinoInfo.MaxTransferCredit, casinoInfo.IsAutoReset, ip, uplineId, modifiedBy, fromSystem);

            // Update PT Casino Game
            
            // Update Betting Limit Casino Game

            // Update PT Mini Game

            // Update Betting Limit Mini Game

            message += errMsg;

            return new Result(null, message);
        }

        private bool DoUpdateCSStatusWithoutSync(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, out string errMsg)
        {
            return this.casino.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
        }

        private DataTable GameKindsDataTableMember()
        {
            DataTable dt = new DataTable();
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
            DataTable dt = new DataTable();
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
            DataTable dt = new DataTable();
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
            DataTable dt = new DataTable();
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
            DataTable dt = new DataTable();
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

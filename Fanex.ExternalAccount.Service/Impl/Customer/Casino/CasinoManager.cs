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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using CasinoLibrary.SBAgent;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino;
    using Fanex.ExternalAccount.DTO.Customer.Casino;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoManager
    {
        private readonly CasinoDao casino = new CasinoDao();
        private static CasinoManager instance = null;
        
        public static CasinoManager Instance
        {
            get
            {
                return CasinoManager.instance ?? new CasinoManager();
            }
        }

        public List<GameKindDetail> GetGameKinds()
        {
            List<GameKindDetail> lsGameKind = new List<GameKindDetail>();
            BasicInfo _csBasicInfo = new BasicInfo();
            DataTable dtGameKind = _csBasicInfo.GetGameKind(null);
            foreach (DataRow row in dtGameKind.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        public bool GetCasinoStatus(int custid)
        {
            return casino.GetCasinoStatus(custid);
        }

        public bool UpdateCSStatus(int custid, bool bSyncCS, bool bDisabledCS, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            return casino.UpdateCSStatus(custid, bSyncCS, bDisabledCS, userId, ip, subAccId, lang, username, subUsername);
        }

        public bool UpdateCSStatus(int custid, bool bSyncCS, bool bDisabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            string errMsg = "";
            return this.UpdateCSStatus(custid, bSyncCS, bDisabledCS, isUpdDownline, userId, ip, subAccId, lang, username, subUsername, out errMsg);
        }

        public bool UpdateCSStatus(int custid, bool bSyncCS, bool bDisabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, string username, string subUsername, out string errMsg)
        {
            bool status = this.DoUpdateCSStatusWithoutSync(custid, bSyncCS, bDisabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
            if (!status) return false;

            Customer cust = new Customer();
            return cust.UpdateCustEnableCasino(custid, !bDisabledCS, isUpdDownline, ip, custid, subAccId == 0 ? username : subUsername, 2);
        }

        public bool UpdateCSStatusWithoutSync(int custid, bool bSyncCS, bool bDisabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang)
        {
            string errMsg = "";
            return DoUpdateCSStatusWithoutSync(custid, bSyncCS, bDisabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
        }

        private bool DoUpdateCSStatusWithoutSync(int custid, bool bSyncCS, bool bDisabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, out string errMsg)
        {
            return casino.DoUpdateCSStatusWithoutSync(custid, bSyncCS, bDisabledCS, isUpdDownline, userId, ip, subAccId, lang, out errMsg);
        }

        /// <summary>
        /// Initialize basics property of an object from posted values of a form. Be used in new customer function.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public CasinoCustomer GetCustInfoIBC(int custid, int roleId, int currencyId, string currency, Hashtable r)
        {
            CasinoCustomer _custSetting = new CasinoCustomer();

            _custSetting.UserName = Utils.ToString(r["username"]);
            _custSetting.UserPwd = Utils.ToString(r["userpwd"]);
            _custSetting.FirstName = Utils.ToString(r["firstname"]);
            _custSetting.LastName = Utils.ToString(r["lastname"]);
            _custSetting.Phone = Utils.ToString(r["phone"]);
            _custSetting.MobilePhone = Utils.ToString(r["mobilephone"]);
            _custSetting.Fax = Utils.ToString(r["fax"]);
            _custSetting.Email = Utils.ToString(r["email"]);
            _custSetting.RoleId = roleId - 1;
            _custSetting.Currency = currencyId;
            _custSetting.CurrencyName = currency;
            _custSetting.Closed = Utils.ToBoolean(r["closed"]);
            _custSetting.Suspend = Utils.ToBoolean(r["suspend"]);

            _custSetting.SIBCCustID = roleId == 4 ? custid : 0;
            _custSetting.MIBCCustID = roleId == 3 ? custid : 0;
            _custSetting.AIBCCustID = roleId == 2 ? custid : 0;

            _custSetting.CasinoSync = false;
            _custSetting.CasinoDisabled = false;
            _custSetting.IBCCustID = custid;

            return _custSetting;
        }

        public List<GameKindDetail> GetGameKinds(string casinoNo)
        {
            List<GameKindDetail> lsGameKind = new List<GameKindDetail>();
            BasicInfo _csBasicInfo = new BasicInfo();
            DataTable dtGameKind = _csBasicInfo.GetGameKind(casinoNo);
            foreach (DataRow row in dtGameKind.Rows)
            {
                GameKindDetail gameKind = new GameKindDetail(Utils.ToInt32(row["CodeID"]), Utils.ToString(row["CodeNo"]), Utils.ToString(row[Lang("CodeName")]), Utils.ToString(row["CodeSName"]));
                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        public List<GameKindDetail> GetGameKinds(int custId)
        {
            List<GameKindDetail> lsGameKind = new List<GameKindDetail>();
            Customer _cust = new Customer();
            DataTable dtGameKind = _cust.GetCustGameKind(custId);
            foreach (DataRow row in dtGameKind.Rows)
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
                // gameKind.MinMPositionTaking = Utils.ToDouble(row["MinMPositionTaking"]);
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
                // gameKind.MinMPositionTakingUpline = Utils.ToDouble(row["MinMPositionTaking"]);
                gameKind.MinPositionTakingUpline = Utils.ToDouble(row["UplineMinPositionTaking"]);
                gameKind.MPositionTakingUpline = Utils.ToDouble(row["UplineMPositionTaking"]);
                gameKind.SMAgentRollingRateUpline = Utils.ToDouble(row["UplineSMAgentRollingRate"]);
                gameKind.SPositionTakingUpline = Utils.ToDouble(row["UplineSPositionTaking"]);

                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        /// <summary>
        /// Get customer PT when super edit member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<GameKindDetail> GetGameKinds_EditMember(int memberId)
        {
            List<GameKindDetail> lsGameKind = new List<GameKindDetail>();
            Customer _cust = new Customer();
            DataTable dtGameKind = _cust.GetCustGameKind_EditMember(memberId);
            foreach (DataRow row in dtGameKind.Rows)
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

                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        //Get customer PT by game type when super edit member
        public List<GameTypeDetail> GetGameTypes_EditMember(int memberId, int gameKindID)
        {
            List<GameTypeDetail> lsGameType = new List<GameTypeDetail>();
            Customer _cust = new Customer();
            DataTable dtGameType = _cust.GetCustGameType_EditMember(memberId, gameKindID);
            foreach (DataRow row in dtGameType.Rows)
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

                lsGameType.Add(gameType);
            }
            return lsGameType;
        }

        /// <summary>
        /// Get game type list of a certain game kind
        /// </summary>
        /// <param name="gameKindID">ID of game kind</param>
        /// <returns>List GameTypeDetail</returns>
        public List<GameTypeDetail> GetGameType(int gameKindID)
        {
            List<GameTypeDetail> lsGameType = new List<GameTypeDetail>();
            BasicInfo _csBasicInfo = new BasicInfo();
            DataTable dtGameType = _csBasicInfo.GetGameType(gameKindID);
            foreach (DataRow row in dtGameType.Rows)
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
                lsGameType.Add(gameType);
            }
            return lsGameType;
        }

        public List<GameTypeDetail> GetGameType(int custId, int gameKindID)
        {
            List<GameTypeDetail> lsGameType = new List<GameTypeDetail>();
            Customer _cust = new Customer();
            DataTable dtGameType = _cust.GetCustGameType(custId, gameKindID);
            foreach (DataRow row in dtGameType.Rows)
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
                //gameType.MinMPositionTaking = Utils.ToDouble(row["MinMPositionTaking"]);
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
                //gameType.MinMPositionTaking = Utils.ToDouble(row["MinMPositionTaking"]);
                gameType.MinPositionTakingUpline = Utils.ToDouble(row["UplineMinPositionTaking"]);
                gameType.MPositionTakingUpline = Utils.ToDouble(row["UplineMPositionTaking"]);
                gameType.SMAgentRollingRateUpline = Utils.ToDouble(row["UplineSMAgentRollingRate"]);
                gameType.SPositionTakingUpline = Utils.ToDouble(row["UplineSPositionTaking"]);

                lsGameType.Add(gameType);
            }
            return lsGameType;
        }

        /// <summary>
        /// Get Game Kind of Master Edit Member
        /// </summary>
        /// <param name="memberid"></param>
        /// <returns></returns>

        public List<GameKindDetail> GetGameKinds_MasterEditMember(int memberid)
        {
            List<GameKindDetail> lsGameKind = new List<GameKindDetail>();
            Customer _cust = new Customer();
            DataTable dtGameKind = _cust.GetCustGameKind(memberid);
            foreach (DataRow row in dtGameKind.Rows)
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

                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        /// <summary>
        /// Get Game Types of Master Edit Member
        /// </summary>
        /// <param name="memberid"></param>
        /// <param name="gameKindID"></param>
        /// <returns></returns>
        public List<GameTypeDetail> GetGameTypes_MasterEditMember(int memberid, int gameKindID)
        {
            List<GameTypeDetail> lsGameType = new List<GameTypeDetail>();
            Customer _cust = new Customer();
            DataTable dtGameType = _cust.GetCustGameType(memberid, gameKindID);
            foreach (DataRow row in dtGameType.Rows)
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
                lsGameType.Add(gameType);
            }
            return lsGameType;
        }

        public List<StakeLevelDetail> GetCustStakeLevels(int custId, int gameTypeID)
        {
            List<StakeLevelDetail> lsStakeLevel = new List<StakeLevelDetail>();
            Customer _cust = new Customer();
            DataTable dtStakeList = _cust.GetCustStakeLevel(custId, gameTypeID);
            foreach (DataRow row in dtStakeList.Rows)
            {
                StakeLevelDetail stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = Utils.ToBoolean(row["IsChecked"]);
                lsStakeLevel.Add(stake);
            }
            return lsStakeLevel;
        }

        public List<StakeLevelDetail> GetGameTypeStakeLevels(int gameTypeID, int currencyId)
        {
            //int custId = 1267168;
            List<StakeLevelDetail> lsStakeLevel = new List<StakeLevelDetail>();
            BasicInfo bi = new BasicInfo();
            DataTable dtStakeList = bi.GetGameTypeStakeLevel(gameTypeID, currencyId);
            foreach (DataRow row in dtStakeList.Rows)
            {
                StakeLevelDetail stake = new StakeLevelDetail();
                stake.StakeLevelID = Utils.ToInt32(row["StakeLevelID"]);
                stake.StakeLevelName = Utils.ToString(row["StakeLevelName"]);
                stake.IsChecked = true;
                lsStakeLevel.Add(stake);
            }
            return lsStakeLevel;
        }

        public List<StakeLevelDetail> GetGameTypeStakeLevels(int gameTypeID, string currencyName)
        {
            throw new NotImplementedException();
        }

        public CasinoCustomer GetCustSettingCS(int custId)
        {
            Customer _cust = new Customer();
            DataTable dtCustomer = _cust.GetCustSetting(custId);
            DataRow row = null;
            CasinoCustomer _custSetting = new CasinoCustomer();
            if (dtCustomer.Rows.Count > 0)
            {
                row = dtCustomer.Rows[0];
                _custSetting.IsEnablePT = Utils.ToBoolean(row["IsEnablePT"]);
                _custSetting.TotalWinLoss = Utils.ToDouble(row["ActualMaxTransfer"]);
                _custSetting.MaxWin = Utils.ToDouble(row["MaxWin"]);
                _custSetting.MaxLose = Utils.ToDouble(row["MaxLose"]);
                _custSetting.MaxTransferCredit = Utils.ToDouble(row["MaxTransferCredit"]);
                _custSetting.IsMaxWin = Utils.ToBoolean(row["IsMaxWin"]);
                _custSetting.IsMaxLose = Utils.ToBoolean(row["IsMaxLose"]);
                _custSetting.PTByGameType = Utils.ToBoolean(row["PTByGameType"]);
                _custSetting.RoleId = Utils.ToInt32(row["RoleId"]);
                _custSetting.UpperCustID = Utils.ToInt32(row["UpperCustId"]);
                _custSetting.IsAutoReset = Utils.ToBoolean(row["IsAutoReset"]);

                // Get max transfer information
                if (_custSetting.UpperCustID != 0)
                {
                    DataTable dt = _cust.GetDefaultMaxTransfer(_custSetting.UpperCustID, 0, false);
                    _custSetting.MaxTransferCreditLimit = Utils.ToDouble((dt.Rows[0]["MaxLimit"]));
                    _custSetting.MinTransferCreditLimit = Utils.ToDouble((dt.Rows[0]["MinLimit"]));

                    _custSetting.DefaultTransferCredit = Utils.ToDouble((dt.Rows[0]["DefMaxTransferCredit"]));
                }

                _custSetting.NoCSData = false;
            }
            return _custSetting;
        }

        //get customer setting from Casino and add to AGE customer info
        public void GetCustSettingCS(ref CasinoCustomer _custIBC)
        {
            Customer _cust = new Customer();
            DataTable dtCustomer = _cust.GetCustSetting(_custIBC.IBCCustID);
            DataRow row = null;
            if (dtCustomer != null && dtCustomer.Rows.Count > 0)
            {
                row = dtCustomer.Rows[0];
                _custIBC.IsEnablePT = Utils.ToBoolean(row["IsEnablePT"]);
                _custIBC.TotalWinLoss = Utils.ToDouble(row["ActualMaxTransfer"]);
                _custIBC.MaxWin = Utils.ToDouble(row["MaxWin"]);
                _custIBC.MaxLose = Utils.ToDouble(row["MaxLose"]);

                _custIBC.MaxTransferCredit = Utils.ToDouble(row["MaxTransferCredit"]);

                _custIBC.IsMaxWin = Utils.ToBoolean(row["IsMaxWin"]);
                _custIBC.IsMaxLose = Utils.ToBoolean(row["IsMaxLose"]);
                _custIBC.PTByGameType = Utils.ToBoolean(row["PTByGameType"]);
                _custIBC.RoleId = Utils.ToInt32(row["RoleId"]);
                _custIBC.UpperCustID = Utils.ToInt32(row["UpperCustId"]);
                _custIBC.IsAutoReset = Utils.ToBoolean(row["IsAutoReset"]);

                // Get max transfer information
                if (_custIBC.UpperCustID != 0)
                {
                    DataTable dt = _cust.GetDefaultMaxTransfer(_custIBC.UpperCustID, 0, false);
                    _custIBC.MaxTransferCreditLimit = Utils.ToDouble((dt.Rows[0]["MaxLimit"]));
                    _custIBC.MinTransferCreditLimit = Utils.ToDouble((dt.Rows[0]["MinLimit"]));

                    _custIBC.DefaultTransferCredit = Utils.ToDouble((dt.Rows[0]["DefMaxTransferCredit"]));
                }

                _custIBC.NoCSData = false;
            }
        }

        public static string Lang(string original)
        {
            //switch (CurrentSession.Instance.Language)
            //{
            //    case "en-US": return original + "_En";
            //    case "ja-JP": return original + "_Jp";
            //    case "zh-TW": return original + "_Ct"; // Traditional
            //    case "zh-CN": return original + "_Cs"; // Simplified
            //    case "th-TH": return original + "_Th";
            //    case "ko-KR": return original + "_Ko";
            //}

            return original + "_En";
        }

    }
}

// --------------------------------------------------------------------------
// <copyright file="ReportDAO.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Report;

    /// <summary>
    /// Report data access object.
    /// </summary>
    public class ReportDAO : DAOBase, IReportDAO
    {
        #region BalanceUpDay

        public DataTable Age_SelBalanceUpDay()
        {
            using (var db = new ObjectDb("Age_SelBalanceUpDay"))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        #endregion BalanceUpDay

        #region Common Report

        /// <summary>
        /// Selects user info.
        /// </summary>
        /// <param name="groupname">Group name.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="userName">Customer name.</param>
        /// <param name="isGetUplineInfo">Gets upper info or not.</param>
        /// <returns>DataTable object.</returns>
        public DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            using (var db = new ObjectDb("EAS_CustomerSelUserInfo"))
            {
                if (custId != 0)
                {
                    db.AddInParameter("custid", DbType.Int32, custId);
                }

                db.AddInParameter("groupname", DbType.String, groupname);
                db.AddInParameter("username", DbType.String, userName);
                db.AddInParameter("isgetuplineinfo", DbType.Boolean, isGetUplineInfo);

                var ds = db.ExecuteDataSet();

                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Searches user info.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="username">Customer username.</param>
        /// <param name="groupname">Group name.</param>
        /// <returns>List of suggested usernames.</returns>
        public List<SuggestedUserName> EAS_CustomerSelUserSearch(int custId, string username, string groupname)
        {
            var entityList = new List<SuggestedUserName>();
            using (var db = new ObjectDb("EAS_CustomerSelUserSearch"))
            {
                db.AddInParameter("custid", DbType.Int32, custId);
                db.AddInParameter("username", DbType.String, username);
                db.AddInParameter("groupname", DbType.String, groupname);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        var updated = false;

                        while (reader.Read())
                        {
                            if (!updated)
                            {
                                // Update User Search
                                using (var updUserSearch = new ObjectDb("EAS_CustomerUpdUserSearch"))
                                {
                                    updUserSearch.AddInParameter("userid", DbType.Int32, custId);
                                    updUserSearch.AddInParameter("search", DbType.String, username.ToUpper());
                                    updUserSearch.AddInParameter("groupname", DbType.String, groupname);

                                    updUserSearch.ExecuteNonQuery();
                                }

                                updated = true;
                            }

                            entityList.Add(new SuggestedUserName { Username = reader.To<string>("username") });
                        }
                    }
                }
            }

            return entityList;
        }

        #endregion Common Report

        #region Daily WinLoss

        /// <summary>
        /// Gets daily win loss in 14 days.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="groupname">Group name.</param>
        /// <returns>List of daily win loss object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com14(DateTime winlostDate, int custId, string groupname)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("groupname", groupname);
            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com14", parameters);
        }

        /// <summary>
        /// Gets daily win loss report for commission.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="groupname">Group name.</param>
        /// <returns>List of daily win loss object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(DateTime winlostDate, int custId, string groupname)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("groupname", groupname);
            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com", parameters);
        }

        /// <summary>
        /// Gets daily win loss report in 14 days for super.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="masterId">Master id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg14(DateTime winlostDate, int custId, int masterId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg14", parameters);
        }

        /// <summary>
        /// Gets daily win loss report for super.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="masterId">Master id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(DateTime winlostDate, int custId, int masterId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg", parameters);
        }

        /// <summary>
        /// Gets daily win loss report.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg14(DateTime winlostDate, int custId, int agentId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg14", parameters);
        }

        /// <summary>
        /// Gets daily win loss report.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(DateTime winlostDate, int custId, int agentId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg", parameters);
        }

        /// <summary>
        /// Gets daily win loss report.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="memberId">Member id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag14(DateTime winlostDate, int custId, int memberId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag14", parameters);
        }

        /// <summary>
        /// Gets daily win loss report.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="memberId">Member id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(DateTime winlostDate, int custId, int memberId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("winlostdate", winlostDate);
            parameters.Add("custid", custId);
            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return this.GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag", parameters);
        }

        #endregion Daily WinLoss

        #region WinLoss Detail

        /// <summary>
        /// Gets win loss detail for commission.
        /// </summary>
        /// <param name="groupname">Group name.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date time.</param>
        /// <param name="filterParams">Filter conditions.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <returns>List object.</returns>
        public List<WinLossDetail> EAS_Rpt_WinLossDetail_Com(
            string groupname, 
            int custId,
            DateTime fromDate,
            DateTime toDate, 
            ICollection<KeyValuePair<string, bool>> filterParams,
            DateTime balanceUpDay)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("groupname", groupname);
            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);
            foreach (KeyValuePair<string, bool> element in filterParams)
            {
                parameters.Add(element.Key, element.Value);
            }

            return this.GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_Com", parameters);
        }

        /// <summary>
        /// Gets win loss detail.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="masterId">Master id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date time.</param>
        /// <param name="filterParams">Filter conditions.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <returns>List object.</returns>
        public List<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(
            int custId, 
            int masterId, 
            DateTime fromDate, 
            DateTime toDate, 
            ICollection<KeyValuePair<string, bool>> filterParams,
            DateTime balanceUpDay)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("custid", custId);
            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);

            foreach (KeyValuePair<string, bool> element in filterParams)
            {
                parameters.Add(element.Key, element.Value);
            }

            return this.GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_SAg", parameters);
        }

        /// <summary>
        /// Gets win loss detail.
        /// </summary>        
        /// <param name="custId">Customer id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date time.</param>
        /// <param name="filterParams">Filter conditions.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <returns>List object.</returns>
        public List<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(
            int custId, 
            int agentId,
            DateTime fromDate,
            DateTime toDate, 
            ICollection<KeyValuePair<string, bool>> filterParams,
            DateTime balanceUpDay)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("custid", custId);
            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);

            foreach (KeyValuePair<string, bool> element in filterParams)
            {
                parameters.Add(element.Key, element.Value);
            }

            return this.GetCommonWinLossDetailMAg("EAS_Rpt_WinLossDetail_MAg", parameters);
        }

        /// <summary>
        /// Gets win loss detail.
        /// </summary>        
        /// <param name="custId">Customer id.</param>
        /// <param name="memberId">Member id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date time.</param>
        /// <param name="filterParams">Filter conditions.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <returns>List object.</returns>
        public List<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(
            int custId,
            int memberId,
            DateTime fromDate, 
            DateTime toDate, 
            ICollection<KeyValuePair<string, bool>> filterParams, 
            DateTime balanceUpDay)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("custid", custId);
            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);

            foreach (KeyValuePair<string, bool> element in filterParams)
            {
                parameters.Add(element.Key, element.Value);
            }

            return this.GetCommonWinLossDetailAg("EAS_Rpt_WinLossDetail_Ag", parameters);
        }

        public ArrayList EAS_Rpt_WinLossDaily_SortingOrder(string groupname)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("groupname", groupname);
            return this.GetSortingOrderList("EAS_Rpt_WinLossDaily_SortingOrder", parameters);
        }

        #endregion WinLoss Detail

        #region Common WinLoss Detail

        /// <summary>
        /// Gets commission win loss detail.
        /// </summary>
        /// <param name="spname">Procedure name.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <returns>List object.</returns>
        private List<WinLossDetail> GetCommonWinLossDetail(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetail>();

            using (IDataReader reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        WinLossDetail entity = this.CreateWinLossDetailEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Creates win loss detail entity.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <returns>WinLossDetail object.</returns>        
        private WinLossDetail CreateWinLossDetailEntity(IDataReader reader)
        {
            var entity = new WinLossDetail
            {
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                Turnover = reader.ToDouble("Turnover"),
                NetTurnover = reader.ToDouble("TurnoverNoDraw"),
                GrossComm = reader.ToDouble("GrossComm"),
                MemberWinlost = reader.ToDouble("MemberWinlost"),
                PlayerComm = reader.ToDouble("PlayerComm"),
                MemberTotal = reader.ToDouble("MemberTotal"),
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgComm = reader.ToDouble("AgComm"),
                AgTotal = reader.ToDouble("AgTotal"),
                MAgWinlost = reader.ToDouble("MAgWinlost"),
                MAgComm = reader.ToDouble("MAgComm"),
                MAgTotal = reader.ToDouble("MAgTotal"),
                SAgWinlost = reader.ToDouble("SAgWinlost"),
                SAgComm = reader.ToDouble("SAgComm"),
                SAgTotal = reader.ToDouble("SAgTotal"),
                Company = reader.ToDouble("Company"),
            };

            return entity;
        }

        /// <summary>
        /// Gets common win loss detail.
        /// </summary>
        /// <param name="spname">Procedure name.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <returns>List object.</returns>
        private List<WinLossDetailMAg> GetCommonWinLossDetailMAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailMAg>();

            using (IDataReader reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        WinLossDetailMAg entity = this.CreateWinLossDetailMAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Creates win loss detail entity.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <returns>WinLossDetailMAg object.</returns>
        private WinLossDetailMAg CreateWinLossDetailMAgEntity(IDataReader reader)
        {
            var entity = new WinLossDetailMAg
            {
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                Turnover = reader.ToDouble("Turnover"),
                NetTurnover = reader.ToDouble("TurnoverNoDraw"),
                GrossComm = reader.ToDouble("GrossComm"),
                MemberWinlost = reader.ToDouble("MemberWinlost"),
                PlayerComm = reader.ToDouble("PlayerComm"),
                MemberTotal = reader.ToDouble("MemberTotal"),
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgComm = reader.ToDouble("AgComm"),
                AgTotal = reader.ToDouble("AgTotal"),
                MAgWinlost = reader.ToDouble("MAgWinlost"),
                MAgComm = reader.ToDouble("MAgComm"),
                MAgTotal = reader.ToDouble("MAgTotal"),
                Company = reader.ToDouble("Company"),
            };

            return entity;
        }

        /// <summary>
        /// Gets common win loss detail.
        /// </summary>
        /// <param name="spname">Procedure name.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <returns>List object.</returns>
        private List<WinLossDetailAg> GetCommonWinLossDetailAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailAg>();

            using (IDataReader reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        WinLossDetailAg entity = this.CreateWinLossDetailAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Creates win loss detail entity.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <returns>List object.</returns>
        private WinLossDetailAg CreateWinLossDetailAgEntity(IDataReader reader)
        {
            var entity = new WinLossDetailAg
            {
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                Turnover = reader.ToDouble("Turnover"),
                NetTurnover = reader.ToDouble("TurnoverNoDraw"),
                GrossComm = reader.ToDouble("GrossComm"),
                MemberWinlost = reader.ToDouble("MemberWinlost"),
                PlayerComm = reader.ToDouble("PlayerComm"),
                MemberTotal = reader.ToDouble("MemberTotal"),
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgComm = reader.ToDouble("AgComm"),
                AgTotal = reader.ToDouble("AgTotal"),
                Company = reader.ToDouble("Company"),
            };

            return entity;
        }

        /// <summary>
        /// Gets sorted list.
        /// </summary>
        /// <param name="spname">Procedure name.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <returns>List object.</returns>
        private ArrayList GetSortingOrderList(string spname, Hashtable parameters)
        {
            var entityList = new ArrayList();
            using (IDataReader reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        entityList.Add(reader["custid"]);
                    }
                }
            }

            return entityList;
        }

        #endregion Common WinLoss Detail

        #region Common WinLoss Daily

        /// <summary>
        /// Gets common win loss daily.
        /// </summary>
        /// <param name="spname">Procedure name.</param>
        /// <param name="parameters">Parameter values.</param>
        /// <returns>List object.</returns>
        private List<DailyWinLoss> GetCommonWinLossDaily(string spname, Hashtable parameters)
        {
            var entityList = new List<DailyWinLoss>();
            int num = 1;

            using (IDataReader reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        DailyWinLoss entity = this.CreateWinLossDailyEntity(reader, num);
                        entityList.Add(entity);
                        num++;
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Creates win loss detail entity.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <param name="num">Order number.</param>
        /// <returns>List object.</returns>
        private DailyWinLoss CreateWinLossDailyEntity(IDataReader reader, int num)
        {
            var entity = new DailyWinLoss
            {
                No = num,
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                TotalTO = reader.ToDouble("totalTO"),
                ComTO = reader.ToDouble("ComTO"),
                ComTOPercent = reader.ToDouble("totalTO") != 0 ? reader.ToDouble("ComTO") * 100 / reader.ToDouble("totalTO") : 0,
                SAgTO = reader.ToDouble("SAgTO"),
                SAgTOPercent = reader.ToDouble("totalTO") != 0 ? reader.ToDouble("SAgTO") * 100 / reader.ToDouble("totalTO") : 0,
                MAgTO = reader.ToDouble("MAgTO"),
                MAgTOPercent = reader.ToDouble("totalTO") != 0 ? reader.ToDouble("MAgTO") * 100 / reader.ToDouble("totalTO") : 0,
                AgTO = reader.ToDouble("AgTO"),
                AgTOPercent = reader.ToDouble("totalTO") != 0 ? reader.ToDouble("AgTO") * 100 / reader.ToDouble("totalTO") : 0,
                ComWinlost = reader.ToDouble("ComWinlost"),
                ComWinlostPercent = reader.ToDouble("ComTO") != 0 ? reader.ToDouble("ComWinlost") * 100 / reader.ToDouble("ComTO") : 0,
                SAgWinlost = reader.ToDouble("SAgWinlost"),
                SAgWinlostPercent = reader.ToDouble("SAgTO") != 0 ? reader.ToDouble("SAgWinlost") * 100 / reader.ToDouble("SAgTO") : 0,
                MAgWinlost = reader.ToDouble("MAgWinlost"),
                MAgWinlostPercent = reader.ToDouble("MAgTO") != 0 ? reader.ToDouble("MAgWinlost") * 100 / reader.ToDouble("MAgTO") : 0,
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgWinlostPercent = reader.ToDouble("AgTO") != 0 ? reader.ToDouble("AgWinlost") * 100 / reader.ToDouble("AgTO") : 0
            };

            return entity;
        }

        #endregion Common WinLoss Daily
    }
}
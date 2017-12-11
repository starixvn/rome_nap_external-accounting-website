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
    using Logging.Extensions.Database;

    public class ReportDAO : DAOBase, IReportDAO
    {
        #region BalanceUpDay

        public DataTable Age_SelBalanceUpDay()
        {
            using (var db = new ObjectDb("Age_SelBalanceUpDay"))
            {
                try
                {
                    return db.ExecuteDataSet().Tables[0];
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        #endregion BalanceUpDay

        #region Common Report

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

                try
                {
                    var ds = db.ExecuteDataSet();

                    return ds == null || ds.Tables.Count == 0
                        ? null :
                        ds.Tables[0];
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        public List<SuggestedUserName> EAS_CustomerSelUserSearch(int custId, string username, string groupname)
        {
            var entityList = new List<SuggestedUserName>();
            using (var db = new ObjectDb("EAS_CustomerSelUserSearch"))
            {
                db.AddInParameter("custid", DbType.Int32, custId);
                db.AddInParameter("username", DbType.String, username);
                db.AddInParameter("groupname", DbType.String, groupname);

                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            var updated = false;

                            while (reader.Read())
                            {
                                if (!updated)
                                {
                                    using (var updUserSearch = new ObjectDb("EAS_CustomerUpdUserSearch"))
                                    {
                                        updUserSearch.AddInParameter("userid", DbType.Int32, custId);
                                        updUserSearch.AddInParameter("search", DbType.String, username.ToUpperInvariant());
                                        updUserSearch.AddInParameter("groupname", DbType.String, groupname);

                                        try
                                        {
                                            updUserSearch.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new DatabaseException(ex.Message, ex, db.DbSetting);
                                        }
                                    }

                                    updated = true;
                                }

                                entityList.Add(new SuggestedUserName { Username = reader.To<string>("username") });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return entityList;
        }

        #endregion Common Report

        #region Daily WinLoss

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

        public List<WinLossDetail> EAS_Rpt_WinLossDetail_Com(
            string groupname, 
            int custId,
            DateTime fromDate,
            DateTime toDate, 
            string productIds,
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
            parameters.Add("productids", productIds);

            return this.GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_Com", parameters);
        }

        public List<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(
            int custId, 
            int masterId, 
            DateTime fromDate, 
            DateTime toDate, 
            string productIds,
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
            parameters.Add("productids", productIds);

            return this.GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_SAg", parameters);
        }

        public List<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(
            int custId, 
            int agentId,
            DateTime fromDate,
            DateTime toDate, 
            string productIds,
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
            parameters.Add("productids", productIds);

            return this.GetCommonWinLossDetailMAg("EAS_Rpt_WinLossDetail_MAg", parameters);
        }

        public List<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(
            int custId,
            int memberId,
            DateTime fromDate, 
            DateTime toDate, 
            string productIds, 
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
            parameters.Add("productids", productIds);

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
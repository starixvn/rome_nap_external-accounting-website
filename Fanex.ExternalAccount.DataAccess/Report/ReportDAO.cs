namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using Base;
    using Common.Extensions;
    using Data;
    using DTO.Customer;
    using DTO.Report;
    using Interface.Report;
    using Utilities;

    public class ReportDao : DAOBase, IReportDao
    {
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

                return ds?.Tables.Count > 0 ? ds.Tables[0] : null;
            }
        }

        public LineInfo GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            LineInfo customerInfo = null;

            using (var db = new ObjectDb("EAS_CustomerSelUserInfo"))
            {
                if (custId != 0)
                {
                    db.AddInParameter("custid", DbType.Int32, custId);
                }

                db.AddInParameter("groupname", DbType.String, groupname);
                db.AddInParameter("username", DbType.String, userName);
                db.AddInParameter("isgetuplineinfo", DbType.Boolean, isGetUplineInfo);

                using (var reader = db.ExecuteReader())
                {
                    if (reader?.Read() != true)
                    {
                        return customerInfo;
                    }

                    customerInfo = new LineInfo
                    {
                        CustId = Utils.ToInt32(reader[reader.GetOrdinal("custid")]),
                        Username = Utils.ToString(reader[reader.GetOrdinal("username")]),
                        RoleId = Utils.ToInt32(reader[reader.GetOrdinal("roleid")]),
                        SuperId = Utils.ToInt32(reader[reader.GetOrdinal("srecommend")]),
                        SuperName = Utils.ToString(reader[reader.GetOrdinal("SuperName")]),
                        MasterId = Utils.ToInt32(reader[reader.GetOrdinal("mrecommend")]),
                        MasterName = Utils.ToString(reader[reader.GetOrdinal("MasterName")]),
                        AgentId = Utils.ToInt32(reader[reader.GetOrdinal("recommend")]),
                        AgentName = Utils.ToString(reader[reader.GetOrdinal("AgentName")]),
                        HasSecurityCode = Utils.ToBoolean(reader[reader.GetOrdinal("HasSecurityCode")])
                    };
                }
            }

            return customerInfo;
        }

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
                                using (var updUserSearch = new ObjectDb("EAS_CustomerUpdUserSearch"))
                                {
                                    updUserSearch.AddInParameter("userid", DbType.Int32, custId);
                                    updUserSearch.AddInParameter("search", DbType.String,
                                        username.ToUpper(CultureInfo.InvariantCulture));
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

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com14(DateTime winlostDate, int custId, string groupname, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"groupname", groupname},
                { "excludeproductids", excludeProductIds}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(DateTime winlostDate, int custId, string groupname, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"groupname", groupname},
                { "excludeproductids", excludeProductIds}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg14(DateTime winlostDate, int custId, int masterId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(DateTime winlostDate, int custId, int masterId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg14(DateTime winlostDate, int custId, int agentId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(DateTime winlostDate, int custId, int agentId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag14(DateTime winlostDate, int custId, int memberId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(DateTime winlostDate, int custId, int memberId, string excludeProductIds)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId},
                { "excludeproductids", excludeProductIds}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag", parameters);
        }

        public List<WinLossDetail> EAS_Rpt_WinLossDetail_Com(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_Com", parameters);
        }

        public List<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossDetail("EAS_Rpt_WinLossDetail_SAg", parameters);
        }

        public List<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossDetailMAg("EAS_Rpt_WinLossDetail_MAg", parameters);
        }

        public List<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }
            return GetCommonWinLossDetailAg("EAS_Rpt_WinLossDetail_Ag", parameters);
        }

        public ArrayList EAS_Rpt_WinLossDaily_SortingOrder(string groupname)
        {
            var parameters = new Hashtable
            {
                { "groupname", groupname }
            };

            return GetSortingOrderList("EAS_Rpt_WinLossDaily_SortingOrder", parameters);
        }

        public List<PokerGamesWinLossDetail> EAS_Rpt_WinLossDetailPokerGames_Com(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"movebetsday", moveBetsDay},
                {"productids", "33"}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonPokerGamesWinLossDetail("EAS_Rpt_WinLossDetailPokerGames_Com", parameters);
        }

        public List<PokerGamesWinLossDetail> EAS_Rpt_WinLossDetailPokerGames_SAg(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"movebetsday", moveBetsDay},
                {"productids", "33"}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonPokerGamesWinLossDetail("EAS_Rpt_WinLossDetailPokerGames_SAg", parameters);
        }

        public List<PokerGamesWinLossDetailMAg> EAS_Rpt_WinLossDetailPokerGames_MAg(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"movebetsday", moveBetsDay},
                {"productids", "33"}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonPokerGamesWinLossDetailMAg("EAS_Rpt_WinLossDetailPokerGames_MAg", parameters);
        }

        public List<PokerGamesWinLossDetailAg> EAS_Rpt_WinLossDetailPokerGames_Ag(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"movebetsday", moveBetsDay},
                {"productids", "33"}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }
            return GetCommonPokerGamesWinLossDetailAg("EAS_Rpt_WinLossDetailPokerGames_Ag", parameters);
        }

        public DataTable Age_SelBalanceUpDay()
        {
            using (var db = new ObjectDb("Age_SelBalanceUpDay"))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public IEnumerable<int> GetExcludeProductIds(string groupName)
        {
            var productIds = new List<int>();
            var parameters = new Hashtable
            {
                ["groupname"] = groupName
            };

            using (var reader = ExecDataReader("Age_ExcludeProduct_SiteCurrency_Sel", parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        productIds.Add(reader.ToInt("ProductId"));
                    }
                }
            }

            return productIds;
        }

        private List<WinLossDetail> GetCommonWinLossDetail(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetail CreateWinLossDetailEntity(IDataReader reader) => new WinLossDetail
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
            Company = reader.ToDouble("Company")
        };

        private List<WinLossDetailMAg> GetCommonWinLossDetailMAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailMAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailMAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailMAg CreateWinLossDetailMAgEntity(IDataReader reader) => new WinLossDetailMAg
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
            Company = reader.ToDouble("Company")
        };

        private List<WinLossDetailAg> GetCommonWinLossDetailAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailAg CreateWinLossDetailAgEntity(IDataReader reader) => new WinLossDetailAg
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
            Company = reader.ToDouble("Company")
        };

        private ArrayList GetSortingOrderList(string spname, Hashtable parameters)
        {
            var entityList = new ArrayList();
            using (var reader = ExecDataReader(spname, parameters))
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

        private List<DailyWinLoss> GetCommonWinLossDaily(string spname, Hashtable parameters)
        {
            var entityList = new List<DailyWinLoss>();
            var num = 1;

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDailyEntity(reader, num);
                        entityList.Add(entity);
                        num++;
                    }
                }
            }

            return entityList;
        }

        private static DailyWinLoss CreateWinLossDailyEntity(IDataReader reader, int num)
            => new DailyWinLoss
            {
                No = num,
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                TotalTO = reader.ToDouble("totalTO"),
                ComTO = reader.ToDouble("ComTO"),
                ComTOPercent = !reader.ToDouble("totalTO").Equals(0)
                ? reader.ToDouble("ComTO") * 100 / reader.ToDouble("totalTO")
                : 0,
                SAgTO = reader.ToDouble("SAgTO"),
                SAgTOPercent = !reader.ToDouble("totalTO").Equals(0)
                ? reader.ToDouble("SAgTO") * 100 / reader.ToDouble("totalTO")
                : 0,
                MAgTO = reader.ToDouble("MAgTO"),
                MAgTOPercent = !reader.ToDouble("totalTO").Equals(0)
                ? reader.ToDouble("MAgTO") * 100 / reader.ToDouble("totalTO")
                : 0,
                AgTO = reader.ToDouble("AgTO"),
                AgTOPercent = !reader.ToDouble("totalTO").Equals(0)
                ? reader.ToDouble("AgTO") * 100 / reader.ToDouble("totalTO")
                : 0,
                ComWinlost = reader.ToDouble("ComWinlost"),
                ComWinlostPercent = !reader.ToDouble("ComTO").Equals(0)
                ? reader.ToDouble("ComWinlost") * 100 / reader.ToDouble("ComTO")
                : 0,
                SAgWinlost = reader.ToDouble("SAgWinlost"),
                SAgWinlostPercent = !reader.ToDouble("SAgTO").Equals(0)
                ? reader.ToDouble("SAgWinlost") * 100 / reader.ToDouble("SAgTO")
                : 0,
                MAgWinlost = reader.ToDouble("MAgWinlost"),
                MAgWinlostPercent = !reader.ToDouble("MAgTO").Equals(0)
                ? reader.ToDouble("MAgWinlost") * 100 / reader.ToDouble("MAgTO")
                : 0,
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgWinlostPercent = !reader.ToDouble("AgTO").Equals(0)
                ? reader.ToDouble("AgWinlost") * 100 / reader.ToDouble("AgTO")
                : 0
            };

        private List<PokerGamesWinLossDetail> GetCommonPokerGamesWinLossDetail(string spname, Hashtable parameters)
        {
            var entityList = new List<PokerGamesWinLossDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreatePokerGamesWinLossDetailEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static PokerGamesWinLossDetail CreatePokerGamesWinLossDetailEntity(IDataReader reader) => new PokerGamesWinLossDetail
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("NetTurnover"),
            TaxComm = reader.ToDouble("TaxComm"),
            MemberWinLoss = reader.ToDouble("MemberWinLoss"),
            MemberInsuranceGemsWinLoss = reader.ToDouble("InsGemMemberWinLoss"),
            MemberComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
            AgInsuranceGemsWinLoss = reader.ToDouble("InsGemAgWinLoss"),
            AgComm = reader.ToDouble("AgComm"),
            AgTotal = reader.ToDouble("AgTotal"),
            MAgInsuranceGemsWinLoss = reader.ToDouble("InsGemMAgWinLoss"),
            MAgComm = reader.ToDouble("MAgComm"),
            MAgTotal = reader.ToDouble("MAgTotal"),
            SAgInsuranceGemsWinLoss = reader.ToDouble("InsGemSAgWinLoss"),
            SAgComm = reader.ToDouble("SAgComm"),
            SAgTotal = reader.ToDouble("SAgTotal"),
            Company = reader.ToDouble("Company")
        };

        private List<PokerGamesWinLossDetailMAg> GetCommonPokerGamesWinLossDetailMAg(string spname, Hashtable parameters)
        {
            var entityList = new List<PokerGamesWinLossDetailMAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreatePokerGamesWinLossDetailMAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static PokerGamesWinLossDetailMAg CreatePokerGamesWinLossDetailMAgEntity(IDataReader reader) => new PokerGamesWinLossDetailMAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("NetTurnover"),
            TaxComm = reader.ToDouble("TaxComm"),
            MemberWinLoss = reader.ToDouble("MemberWinloss"),
            MemberInsuranceGemsWinLoss = reader.ToDouble("InsGemMemberWinLoss"),
            MemberComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
            AgInsuranceGemsWinLoss = reader.ToDouble("InsGemAgWinLoss"),
            AgComm = reader.ToDouble("AgComm"),
            AgTotal = reader.ToDouble("AgTotal"),
            MAgInsuranceGemsWinLoss = reader.ToDouble("InsGemMAgWinLoss"),
            MAgComm = reader.ToDouble("MAgComm"),
            MAgTotal = reader.ToDouble("MAgTotal"),
            Company = reader.ToDouble("Company")
        };

        private List<PokerGamesWinLossDetailAg> GetCommonPokerGamesWinLossDetailAg(string spname, Hashtable parameters)
        {
            var entityList = new List<PokerGamesWinLossDetailAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreatePokerGamesWinLossDetailAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static PokerGamesWinLossDetailAg CreatePokerGamesWinLossDetailAgEntity(IDataReader reader) => new PokerGamesWinLossDetailAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("NetTurnover"),
            TaxComm = reader.ToDouble("TaxComm"),
            MemberWinLoss = reader.ToDouble("MemberWinLoss"),
            MemberInsuranceGemsWinLoss = reader.ToDouble("InsGemMemberWinLoss"),
            MemberComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
            AgInsuranceGemsWinLoss = reader.ToDouble("InsGemAgWinLoss"),
            AgComm = reader.ToDouble("AgComm"),
            AgTotal = reader.ToDouble("AgTotal"),
            Company = reader.ToDouble("Company")
        };
    }
}
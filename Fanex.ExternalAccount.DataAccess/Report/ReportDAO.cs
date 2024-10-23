namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Base;
    using Common.Extensions;
    using Data;
    using DTO.Customer;
    using DTO.Report;
    using Interface.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using Utilities;

    public partial class ReportDao : DAOBase, IReportDao
    {
        public DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            using (var db = new ObjectDb("EAS_CustomerSelUserInfo"))
            {
                if (custId != 0)
                {
#pragma warning disable S1192 // String literals should not be duplicated
                    db.AddInParameter("custid", DbType.Int32, custId);
#pragma warning restore S1192 // String literals should not be duplicated
                }

                db.AddInParameter("groupname", DbType.String, groupname);
#pragma warning disable S1192 // String literals should not be duplicated
                db.AddInParameter("username", DbType.String, userName);
#pragma warning restore S1192 // String literals should not be duplicated
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

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com14(DateTime winlostDate, int custId, string groupname)
        {
            var parameters = new Hashtable
            {
#pragma warning disable S1192 // String literals should not be duplicated
                {"winlostdate", winlostDate},
#pragma warning restore S1192 // String literals should not be duplicated
                {"groupname", groupname}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(DateTime winlostDate, int custId, string groupname)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"groupname", groupname}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Com", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg14(DateTime winlostDate, int custId, int masterId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(DateTime winlostDate, int custId, int masterId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_SAg", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg14(DateTime winlostDate, int custId, int agentId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(DateTime winlostDate, int custId, int agentId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_MAg", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag14(DateTime winlostDate, int custId, int memberId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag14", parameters);
        }

        public List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(DateTime winlostDate, int custId, int memberId)
        {
            var parameters = new Hashtable
            {
                {"winlostdate", winlostDate},
                {"custid", custId}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetCommonWinLossDaily("EAS_Rpt_WinLossDaily_Ag", parameters);
        }

        public List<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
#pragma warning disable S1192 // String literals should not be duplicated
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
#pragma warning restore S1192 // String literals should not be duplicated
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossDetailExtra("EAS_Rpt_WinLossDetail_Com", parameters);
        }

        public List<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
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
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
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
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
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

        public DataTable Age_SelBalanceUpDay()
        {
            using (var db = new ObjectDb("Age_SelBalanceUpDay"))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public List<ExtraPTModel> EAS_Rpt_ViewExtraPT2(string groupName)
        {
            var entityList = new List<ExtraPTModel>();

            using (var db = new ObjectDb("EAS_Rpt_ViewExtraPT2"))
            {
                db.AddInParameter("GroupName", DbType.String, groupName);

                using (var reader = db.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = CreateExtraPTEntity(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        public static ExtraPTModel CreateExtraPTEntity(IDataReader reader) => new ExtraPTModel
        {
            ProductId = reader.ToInt("ProductID"),
            ExtraPT = reader.ToFloat("ExtraPT")
        };

        private List<WinLossDetailExtra> GetCommonWinLossDetailExtra(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailExtra>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailExtraEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailExtra CreateWinLossDetailExtraEntity(IDataReader reader) => new WinLossDetailExtra
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
#pragma warning disable S1192 // String literals should not be duplicated
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
            ExtraWinLost = reader.ToDouble("ExtraWinLoss"),
            ExtraComm = reader.ToDouble("ExtraComm"),
            ExtraTotal = reader.ToDouble("ExtraTotal"),
            CompanyAfterExtraPT = reader.ToDouble("CompanyAfterExtraPT"),
            Company = reader.ToDouble("Company"),
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
#pragma warning restore S1192 // String literals should not be duplicated
        };

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

        private static WinLossDetail CreateWinLossDetailEntity(IDataReader reader)
            => new WinLossDetail
            {
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                Turnover = reader.ToDouble("Turnover"),
                NetTurnover = reader.ToDouble("TurnoverNoDraw"),
                GrossComm = reader.ToDouble("GrossComm"),
                MemberWinlost = reader.ToDouble("MemberWinlost"),
                PlayerComm = reader.ToDouble("PlayerComm"),
                MemberTotal = reader.ToDouble("MemberTotal"),
#pragma warning disable S1192 // String literals should not be duplicated
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
                RewardAmount = reader.ToDouble("RewardAmount"),
                RewardCurrency = reader.To<string>("RewardCurrency")
#pragma warning restore S1192 // String literals should not be duplicated
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

        private static WinLossDetailMAg CreateWinLossDetailMAgEntity(IDataReader reader)
            => new WinLossDetailMAg
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
                RewardAmount = reader.ToDouble("RewardAmount"),
                RewardCurrency = reader.To<string>("RewardCurrency")
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

        private static WinLossDetailAg CreateWinLossDetailAgEntity(IDataReader reader)
            => new WinLossDetailAg
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
                RewardAmount = reader.ToDouble("RewardAmount"),
                RewardCurrency = reader.To<string>("RewardCurrency")
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
#pragma warning disable S1192 // String literals should not be duplicated
#pragma warning disable S109 // Magic numbers should not be used
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
#pragma warning restore S109 // Magic numbers should not be used
#pragma warning restore S1192 // String literals should not be duplicated
            };


        public List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Com(
            string groupname, 
            int custId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate, 
            DateTime toDate, 
            string productIds, 
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
#pragma warning disable S1192 // String literals should not be duplicated
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
#pragma warning restore S1192 // String literals should not be duplicated
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetCommonWinLossMCDetail("EAS_Rpt_WinLossDetail_Com", parameters);
        }

        public List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_SAg(
            int custId,
            int masterId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate, 
            DateTime toDate,
            string productIds, 
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetCommonWinLossMCDetail("EAS_Rpt_WinLossDetail_SAg", parameters);
        }

        private List<WinLossByCurrencyDetail> GetCommonWinLossMCDetail(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossByCurrencyDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossMCDetailEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossByCurrencyDetail CreateWinLossMCDetailEntity(IDataReader reader) => new WinLossByCurrencyDetail
        {
            CustId = reader.ToInt("custid"),
            Currency = "test",
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("TurnoverNoDraw"),
            GrossComm = reader.ToDouble("GrossComm"),
            MemberWinlost = reader.ToDouble("MemberWinlost"),
            PlayerComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
#pragma warning disable S1192 // String literals should not be duplicated
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
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
#pragma warning restore S1192 // String literals should not be duplicated
        };

        public List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_MAg(
            int custId, 
            int agentId,
            string currencyIds,
            int baseCurrency, 
            DateTime fromDate, 
            DateTime toDate, 
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {

            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetCommonWinLossMCDetailMAg("EAS_Rpt_WinLossDetail_MAg", parameters); 
        }

        public List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Ag(
            int custId, 
            int memberId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML, 
            DateTime balanceUpDay, 
            DateTime moveBetsDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetCommonWinLossMCDetailAg("EAS_Rpt_WinLossDetail_Ag", parameters);
        }

        private List<WinLossByCurrencyDetail> GetCommonWinLossMCDetailMAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossByCurrencyDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossMCDetailMAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossByCurrencyDetail CreateWinLossMCDetailMAgEntity(IDataReader reader) => new WinLossByCurrencyDetail
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
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
        };

        private List<WinLossByCurrencyDetail> GetCommonWinLossMCDetailAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossByCurrencyDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossMCDetailAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossByCurrencyDetail CreateWinLossMCDetailAgEntity(IDataReader reader) => new WinLossByCurrencyDetail
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
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
        };
    }
}
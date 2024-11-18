#pragma warning disable S1192 // String literals should not be duplicated
namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    
    public class ReportByCurrencyDAO : DAOBase, IReportByCurrencyDAO
    {
        private readonly string[] mappingBaseCurrency = { "MYR", "CNY", "THB", "IDR" };
        public List<CurrencyEntity> EAS_GetCurrencyList(string groupName)
        {
            var currencyList = new List<CurrencyEntity>();

            using (var db = new ObjectDb("EAS_GetCurrencyList"))
            {
                db.AddInParameter("groupname", DbType.String, groupName);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cur = new CurrencyEntity
                        {
                            CurrencyId = Utils.ToInt32(reader[reader.GetOrdinal("CurrencyId")]),
                            CurrencyName = Utils.ToString(reader[reader.GetOrdinal("CurrencyName")])
                        };

                        currencyList.Add(cur);
                    }
                }
            }

            return currencyList;
        }

        public WinLossByCurrency EAS_Rpt_WinLossByCurDaily_Com(
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
            var baseCurrencyName = mappingBaseCurrency[baseCurrency - 1];
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay},
                {"basecurrency", baseCurrencyName},
                {"currencyids", currencyIds}
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            return GetWinLossByCurrencyDetail("EAS_Rpt_WinLossDetailByCurrency_Com", parameters);
        }

        public WinLossByCurrency EAS_Rpt_WinLossByCurDaily_SAg(
            string groupname,
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
            var baseCurrencyName = mappingBaseCurrency[baseCurrency - 1];
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay},
                {"basecurrency", baseCurrencyName}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            return GetWinLossByCurrencyDetail("EAS_Rpt_WinLossDetailByCurrency_SAg", parameters);
        }

        public WinLossByCurrencyMAg EAS_Rpt_WinLossByCurDaily_MAg(
            string groupname,
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
            var baseCurrencyName = mappingBaseCurrency[baseCurrency - 1];
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay},
                {"basecurrency", baseCurrencyName}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            return GetWinLossByCurrencyDetailMAg("EAS_Rpt_WinLossDetailByCurrency_MAg", parameters);
        }

        public WinLossByCurrencyAg EAS_Rpt_WinLossByCurDaily_Ag(
            string groupname,
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
            var baseCurrencyName = mappingBaseCurrency[baseCurrency - 1];
            var parameters = new Hashtable
            {
                {"groupname", groupname},
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds},
                {"productdetails", productDetailsXML},
                {"movebetsday", moveBetsDay},
                {"basecurrency", baseCurrencyName}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            return GetWinLossByCurrencyDetailAg("EAS_Rpt_WinLossDetailByCurrency_Ag", parameters);
        }

        private WinLossByCurrency GetWinLossByCurrencyDetail(string spname, Hashtable parameters)
        {
            var details = new List<WinLossByCurrencyDetail>();
            var totals = new List<WinLossByCurrencyDetail>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyDetail(reader);
                        details.Add(entity);
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyTotal(reader);
                        totals.Add(entity);
                    }
                }
            }

            return new WinLossByCurrency
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }

        private static WinLossByCurrencyDetail CreateWinLossByCurrencyDetail(IDataReader reader) => new WinLossByCurrencyDetail
        {
            CustId = reader.ToInt("custid"),
            Currency = reader.To<string>("currencyname"),
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
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
        };

        private static WinLossByCurrencyDetail CreateWinLossByCurrencyTotal(IDataReader reader) => new WinLossByCurrencyDetail
        {
            Username = reader.To<string>("total"),
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
            RewardAmount = reader.ToDouble("RewardAmount"),
            RewardCurrency = reader.To<string>("RewardCurrency")
        };

        private WinLossByCurrencyMAg GetWinLossByCurrencyDetailMAg(string spname, Hashtable parameters)
        {
            var details = new List<WinLossByCurrencyDetailMAg>();
            var totals = new List<WinLossByCurrencyDetailMAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyDetailMAg(reader);
                        details.Add(entity);
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyTotalMAg(reader);
                        totals.Add(entity);
                    }
                }
            }

            return new WinLossByCurrencyMAg
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }
        private static WinLossByCurrencyDetailMAg CreateWinLossByCurrencyDetailMAg(IDataReader reader) => new WinLossByCurrencyDetailMAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Currency = reader.To<string>("currencyname"),
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

        private static WinLossByCurrencyDetailMAg CreateWinLossByCurrencyTotalMAg(IDataReader reader) => new WinLossByCurrencyDetailMAg
        {
            Username = reader.To<string>("total"),
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

        private WinLossByCurrencyAg GetWinLossByCurrencyDetailAg(string spname, Hashtable parameters)
        {
            var details = new List<WinLossByCurrencyDetailAg>();
            var totals = new List<WinLossByCurrencyDetailAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyDetailAg(reader);
                        details.Add(entity);
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        var entity = CreateWinLossByCurrencyTotalAg(reader);
                        totals.Add(entity);
                    }
                }
            }

            return new WinLossByCurrencyAg
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }
        private static WinLossByCurrencyDetailAg CreateWinLossByCurrencyDetailAg(IDataReader reader) => new WinLossByCurrencyDetailAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Currency = reader.To<string>("currencyname"),
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

        private static WinLossByCurrencyDetailAg CreateWinLossByCurrencyTotalAg(IDataReader reader) => new WinLossByCurrencyDetailAg
        {
            Username = reader.To<string>("total"),
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


#pragma warning restore S1192 // String literals should not be duplicated

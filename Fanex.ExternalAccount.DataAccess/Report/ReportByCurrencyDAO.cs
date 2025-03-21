#pragma warning disable S1192 // String literals should not be duplicated
namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Data.Repository;
    using System.Linq;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    
    public class ReportByCurrencyDAO : IReportByCurrencyDAO
    {
        private readonly IDynamicRepository dynamicRepository;
        
        public ReportByCurrencyDAO(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }
        
        public List<CurrencyEntity> EAS_GetCurrencyList(string groupName)
        {
            var criteria = new GetCurrencyListCriteria(groupName);
            return dynamicRepository.Fetch<CurrencyEntity>(criteria).ToList();
        }

        public WinLossByCurrency EAS_Rpt_WinLossByCurDaily_Com(
            string groupname,
            int custId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var criteria = new WinLossByCurrencyComCriteria
            {
                GroupName = groupname,
                CustId = custId > 0 ? custId : 0,
                Currencies = currencies,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            var dataset = dynamicRepository.FetchDataSet(criteria);
            
            return ParseWinLossByCurrencyDataSet(dataset);
        }

        public WinLossByCurrency EAS_Rpt_WinLossByCurDaily_SAg(
            string groupname,
            int custId,
            int masterId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var criteria = new WinLossByCurrencySAgCriteria
            {
                GroupName = groupname,
                CustId = custId,
                MasterId = masterId > 0 ? masterId : (int?)null,
                MemberId = memberId > 0 ? memberId : (int?)null,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            var dataset = dynamicRepository.FetchDataSet(criteria);
            
            return ParseWinLossByCurrencyDataSet(dataset);
        }
        
        public WinLossByCurrencyMAg EAS_Rpt_WinLossByCurDaily_MAg(
            string groupname,
            int custId,
            int agentId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var criteria = new WinLossByCurrencyMAgCriteria
            {
                GroupName = groupname,
                CustId = custId,
                AgentId = agentId > 0 ? agentId : (int?)null,
                MemberId = memberId > 0 ? memberId : (int?)null,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            var dataset = dynamicRepository.FetchDataSet(criteria);
            
            return ParseWinLossByCurrencyMAgDataSet(dataset);
        }      
        

        public WinLossByCurrencyAg EAS_Rpt_WinLossByCurDaily_Ag(
            string groupname,
            int custId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
        {
            var criteria = new WinLossByCurrencyAgCriteria
            {
                GroupName = groupname,
                CustId = custId,
                MemberId = memberId > 0 ? memberId : (int?)null,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            var dataset = dynamicRepository.FetchDataSet(criteria);
            
            return ParseWinLossByCurrencyAgDataSet(dataset);
        }

        private WinLossByCurrency ParseWinLossByCurrencyDataSet(DataSet dataset)
        {
            var details = new List<WinLossByCurrencyDetail>();
            var totals = new List<WinLossByCurrencyDetail>();

            if (dataset != null && dataset.Tables.Count >= 2)
            {
                // Process details table
                foreach (var row in dataset.Tables[0].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetail
                    {
                        CustId = row.Field<int?>("custid") ?? 0,
                        Currency = row.Field<string>("currencyname") ?? string.Empty,
                        Username = row.Field<string>("username") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        MAgWinlost = row.Field<double?>("MAgWinlost") ?? 0,
                        MAgComm = row.Field<double?>("MAgComm") ?? 0,
                        MAgTotal = row.Field<double?>("MAgTotal") ?? 0,
                        SAgWinlost = row.Field<double?>("SAgWinlost") ?? 0,
                        SAgComm = row.Field<double?>("SAgComm") ?? 0,
                        SAgTotal = row.Field<double?>("SAgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    details.Add(entity);
                }

                // Process totals table
                foreach (var row in dataset.Tables[1].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetail
                    {
                        Username = row.Field<string>("total") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        MAgWinlost = row.Field<double?>("MAgWinlost") ?? 0,
                        MAgComm = row.Field<double?>("MAgComm") ?? 0,
                        MAgTotal = row.Field<double?>("MAgTotal") ?? 0,
                        SAgWinlost = row.Field<double?>("SAgWinlost") ?? 0,
                        SAgComm = row.Field<double?>("SAgComm") ?? 0,
                        SAgTotal = row.Field<double?>("SAgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    totals.Add(entity);
                }
            }

            return new WinLossByCurrency
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }

        private WinLossByCurrencyMAg ParseWinLossByCurrencyMAgDataSet(DataSet dataset)
        {
            var details = new List<WinLossByCurrencyDetailMAg>();
            var totals = new List<WinLossByCurrencyDetailMAg>();

            if (dataset != null && dataset.Tables.Count >= 2)
            {
                // Process details table
                foreach (var row in dataset.Tables[0].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetailMAg
                    {
                        CustId = row.Field<int?>("custid") ?? 0,
                        Currency = row.Field<string>("currencyname") ?? string.Empty,
                        Username = row.Field<string>("username") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        MAgWinlost = row.Field<double?>("MAgWinlost") ?? 0,
                        MAgComm = row.Field<double?>("MAgComm") ?? 0,
                        MAgTotal = row.Field<double?>("MAgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    details.Add(entity);
                }

                // Process totals table
                foreach (var row in dataset.Tables[1].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetailMAg
                    {
                        Username = row.Field<string>("total") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        MAgWinlost = row.Field<double?>("MAgWinlost") ?? 0,
                        MAgComm = row.Field<double?>("MAgComm") ?? 0,
                        MAgTotal = row.Field<double?>("MAgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    totals.Add(entity);
                }
            }

            return new WinLossByCurrencyMAg
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }

        private WinLossByCurrencyAg ParseWinLossByCurrencyAgDataSet(DataSet dataset)
        {
            var details = new List<WinLossByCurrencyDetailAg>();
            var totals = new List<WinLossByCurrencyDetailAg>();

            if (dataset != null && dataset.Tables.Count >= 2)
            {
                // Process details table
                foreach (var row in dataset.Tables[0].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetailAg
                    {
                        CustId = row.Field<int?>("custid") ?? 0,
                        Currency = row.Field<string>("currencyname") ?? string.Empty,
                        Username = row.Field<string>("username") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    details.Add(entity);
                }

                // Process totals table
                foreach (var row in dataset.Tables[1].AsEnumerable())
                {
                    var entity = new WinLossByCurrencyDetailAg
                    {
                        Username = row.Field<string>("total") ?? string.Empty,
                        Turnover = row.Field<double?>("Turnover") ?? 0,
                        NetTurnover = row.Field<double?>("TurnoverNoDraw") ?? 0,
                        GrossComm = row.Field<double?>("GrossComm") ?? 0,
                        MemberWinlost = row.Field<double?>("MemberWinlost") ?? 0,
                        PlayerComm = row.Field<double?>("PlayerComm") ?? 0,
                        MemberTotal = row.Field<double?>("MemberTotal") ?? 0,
                        AgWinlost = row.Field<double?>("AgWinlost") ?? 0,
                        AgComm = row.Field<double?>("AgComm") ?? 0,
                        AgTotal = row.Field<double?>("AgTotal") ?? 0,
                        Company = row.Field<double?>("Company") ?? 0,
                        RewardAmount = row.Field<double?>("RewardAmount") ?? 0,
                        RewardCurrency = row.Field<string>("RewardCurrency") ?? string.Empty
                    };
                    totals.Add(entity);
                }
            }

            return new WinLossByCurrencyAg
            {
                WinLossDetails = details,
                WinLossTotals = totals
            };
        }
    }
}


#pragma warning restore S1192 // String literals should not be duplicated
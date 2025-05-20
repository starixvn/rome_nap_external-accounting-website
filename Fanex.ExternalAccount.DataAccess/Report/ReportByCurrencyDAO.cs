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

        public ReportByCurrencyDAO(IDynamicRepository dynamicRepository = null) =>
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();


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
                Currencies = currencies,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            if (custId != 0)
            {
                criteria.CustId = custId;
            }

            var dataset = dynamicRepository.FetchMultiple<WinLossByCurrencyDetail, WinLossByCurrencyDetail>(criteria);

            return new WinLossByCurrency
            {
                WinLossDetails = dataset.Item1.ToList(),
                WinLossTotals = dataset.Item2.ToList()
            };
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

            var dataset = dynamicRepository.FetchMultiple<WinLossByCurrencyDetail, WinLossByCurrencyDetail>(criteria);

            return new WinLossByCurrency
            {
                WinLossDetails = dataset.Item1.ToList(),
                WinLossTotals = dataset.Item2.ToList()
            };
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
                MemberId = memberId > 0 ? memberId : (int?)null,
                AgentId = agentId > 0 ? agentId : (int?)null,
                BaseCurrency = baseCurrencyName,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetails = productDetailsXML,
                BalanceUpDay = balanceUpDay,
                MoveBetsDay = moveBetsDay
            };

            var dataset = dynamicRepository.FetchMultiple<WinLossByCurrencyDetailMAg, WinLossByCurrencyDetailMAg>(criteria);

            return new WinLossByCurrencyMAg
            {
                WinLossDetails = dataset.Item1.ToList(),
                WinLossTotals = dataset.Item2.ToList()
            };
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

            if (memberId != 0)
            {
                criteria.MemberId = memberId;
            }

            var dataset =
                dynamicRepository.FetchMultiple<WinLossByCurrencyDetailAg, WinLossByCurrencyDetailAg>(criteria);

            return new WinLossByCurrencyAg
            {
                WinLossDetails = dataset.Item1.ToList(),
                WinLossTotals = dataset.Item2.ToList()
            };
        }
    }
}


#pragma warning restore S1192 // String literals should not be duplicated
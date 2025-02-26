namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Common.Extensions;
    using DTO.Customer;
    using DTO.Report;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    using Interface.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    public partial class ReportDao : IReportDao
    {
        private IDynamicRepository dynamicRepository;

        public ReportDao() : this(null)
        { }

        public ReportDao(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public LineInfo GetCustomerInfo(GetCustomerInfoCriteria criteria)
            => dynamicRepository.Get<LineInfo>(criteria);

        public IEnumerable<SuggestedUserName> EAS_CustomerSelUserSearch(GetSuggestUsernamesCriteria criteria)
        {
            var result = dynamicRepository.Fetch<SuggestedUserName>(criteria);

            if (result.Any())
            {
                this.UpdateSearchedUserName(new UpdateSuggestUsernameCriteria(criteria.CustId, criteria.UserName.ToUpper(CultureInfo.InvariantCulture), criteria.GroupName));
            }
            
            return result;
        }

        private void UpdateSearchedUserName(UpdateSuggestUsernameCriteria criteria)
            => dynamicRepository.Execute(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(GetCompanyDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(GetSuperDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(GetMasterDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(GetAgentDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(GetSuperWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetailExtra>(criteria);


        public IEnumerable<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(GetMasterWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetail>(criteria);

        public IEnumerable<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(GetAgentWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetailMAg>(criteria);

        public IEnumerable<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(GetMemberWinlossDetailCriteria criteria)
             => dynamicRepository.Fetch<WinLossDetailAg>(criteria);

        public IEnumerable<int> EAS_Rpt_WinLossDaily_SortingOrder(GetSortingOrderCriteria criteria)
            => dynamicRepository.Fetch<int>(criteria);

        public IEnumerable<ExtraPTModel> EAS_Rpt_ViewExtraPT2(GetExtraPTCriteria criteria)
            => dynamicRepository.Fetch<ExtraPTModel>(criteria);

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
                { "groupname", groupname },
                { "fromdate", fromDate },
                { "todate", toDate },
                { "balanceupday", balanceUpDay },
                { "productids", productIds },
                { "productdetails", productDetailsXML },
                { "movebetsday", moveBetsDay }
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
                { "custid", custId },
                { "fromdate", fromDate },
                { "todate", toDate },
                { "balanceupday", balanceUpDay },
                { "productids", productIds },
                { "productdetails", productDetailsXML },
                { "movebetsday", moveBetsDay }
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

        private static WinLossByCurrencyDetail CreateWinLossMCDetailEntity(IDataReader reader) =>
            new WinLossByCurrencyDetail
            {
                CustId = reader.ToInt("custid"),
                Currency = "test",
            Username = reader.ToString("username"),
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
                { "custid", custId },
                { "fromdate", fromDate },
                { "todate", toDate },
                { "balanceupday", balanceUpDay },
                { "productids", productIds },
                { "productdetails", productDetailsXML },
                { "movebetsday", moveBetsDay }
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
                { "custid", custId },
                { "fromdate", fromDate },
                { "todate", toDate },
                { "balanceupday", balanceUpDay },
                { "productids", productIds },
                { "productdetails", productDetailsXML },
                { "movebetsday", moveBetsDay }
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

        private static WinLossByCurrencyDetail CreateWinLossMCDetailMAgEntity(IDataReader reader) =>
            new WinLossByCurrencyDetail
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

        private static WinLossByCurrencyDetail CreateWinLossMCDetailAgEntity(IDataReader reader) =>
            new WinLossByCurrencyDetail
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
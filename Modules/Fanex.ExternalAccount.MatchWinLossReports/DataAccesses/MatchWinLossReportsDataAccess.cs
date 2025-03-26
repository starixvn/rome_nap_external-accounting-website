namespace Fanex.ExternalAccount.MatchWinLossReports.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Criteria;
    using Data;
    using Models;
    //using Fanex.Common.Extensions;
    using System.Linq;
    using Fanex.Data.Repository;
    
    public interface IMatchWinLossReportsDataAccess
    {
        SummaryReportResult GetSummaryReportData(GetSummaryReportDataCriteria criteria);

        DetailReportResult GetDetailReportData(GetDetailReportDataCriteria criteria);

        IEnumerable<SportModel> GetSports(SportsCriteria criteria);

        IEnumerable<LeagueModel> GetLeagues(LeaguesCriteria criteria);

        IEnumerable<CurrencyModel> GetCurrencies(CurrencyCriteria criteria);

        IEnumerable<BetTypeModel> GetBetTypes(BetTypeCriteria criteria);

        IEnumerable<MatchModel> GetMatches(MatchCriteria criteria);
    }

    public class MatchWinLossReportsDataAccess : IMatchWinLossReportsDataAccess
    {
        private readonly IDynamicRepository repository;
        public MatchWinLossReportsDataAccess(IDynamicRepository repository = null) => this.repository = repository ?? new DynamicRepository();
        public SummaryReportResult GetSummaryReportData(GetSummaryReportDataCriteria criteria)
        {
            var matches = new List<ReportRawEntity>();
            var grandTotal = new ReportRawEntity();
            var total = new ReportRawEntity();
            var extraInfos = new List<ReportRawEntity>();
            var totalRows = 0;
            var dbResult = this.repository.FetchMultiple<ReportRawEntity, ReportRawEntity, ReportRawEntity, ReportRawEntity>(criteria);
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var baseInfo = ParseBaseInfo(reader);
            //             baseInfo.KickOffTime = reader.ToDateTime("kickofftime");
            //             baseInfo.KickOffTimeStr = reader.ToString("kickofftime");
            //             baseInfo.LeagueId = reader.ToInt("leagueid");
            //             baseInfo.MatchId = reader.ToInt("matchid");
            //             baseInfo.HomeId = reader.ToInt("homeid");
            //             baseInfo.AwayId = reader.ToInt("awayid");
            //             baseInfo.IsNeutral = reader.ToBool("isneutral");
            //
            //             matches.Add(baseInfo);
            //         }
            //
            //         // total MYR
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             total = ParseBaseInfo(reader);
            //         }
            //
            //         //  system parlay, outright
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             var baseInfo = ParseBaseInfo(reader);
            //             baseInfo.BetTypeId = reader.ToInt("bettype");
            //             extraInfos.Add(baseInfo);
            //         }
            //
            //         //  grand total MYR
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             grandTotal = ParseBaseInfo(reader);
            //         }
            //     }
            //
            //     var outputParams = db.GetOuputParams();
            //     totalRows = outputParams["totalrows"] is DBNull ? 0 : Convert.ToInt16(outputParams["totalrows"]);
            // }

            // return new SummaryReportResult
            // {
            //     Events = matches.ToList(),
            //     GroupLeagues = matches.GroupBy(x => x.LeagueId).Select(x => x.First()).ToList(),
            //     GrandTotal = grandTotal,
            //     Total = total,
            //     SystemParlayOutright = extraInfos,
            //     TotalItems = totalRows
            // };
            return new SummaryReportResult
            {
                Events = dbResult.Item1.ToList(),
                GroupLeagues = dbResult.Item1.GroupBy(x => x.LeagueId).Select(x => x.First()).ToList(),
                Total = dbResult.Item2.Any() ? dbResult.Item2.First() : null,
                SystemParlayOutright = dbResult.Item3.ToList(),
                GrandTotal = dbResult.Item4.First(),
                TotalItems = criteria.TotalRows
            };
        }

        public DetailReportResult GetDetailReportData(GetDetailReportDataCriteria criteria)
        {
            var subTotals = new List<ReportRawEntity>();
            var mains = new List<ReportRawEntity>();
            var total = new ReportRawEntity();
            var grandTotal = new ReportRawEntity();
            var parlayOutright = new List<ReportRawEntity>();
            var totalRows = 0;
            var dbResult = repository.FetchMultiple<ReportRawEntity, ReportRawEntity, ReportRawEntity, ReportRawEntity, ReportRawEntity>(criteria);
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var baseInfo = ParseBaseInfo(reader);
            //             baseInfo.KickOffTime = reader.ToDateTime("kickofftime");
            //             baseInfo.KickOffTimeStr = reader.ToString("kickofftime");
            //             baseInfo.LeagueId = reader.ToInt("leagueid");
            //             baseInfo.MatchId = reader.ToInt("matchid");
            //             baseInfo.HomeId = reader.ToInt("homeid");
            //             baseInfo.AwayId = reader.ToInt("awayid");
            //             baseInfo.IsNeutral = reader.ToBool("isneutral");
            //             subTotals.Add(baseInfo);
            //         }
            //
            //         // main
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             var baseInfo = ParseBaseInfo(reader);
            //             baseInfo.MatchId = reader.ToInt("matchid");
            //             baseInfo.BetTypeId = reader.ToInt("bettype");
            //             mains.Add(baseInfo);
            //         }
            //
            //         // total MYR
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             total = ParseBaseInfo(reader);
            //         }
            //
            //         //  system parlay, outright
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             var baseInfo = ParseBaseInfo(reader);
            //             baseInfo.BetTypeId = reader.ToInt("bettype");
            //             parlayOutright.Add(baseInfo);
            //         }
            //
            //         //  grand total MYR
            //         reader.NextResult();
            //         while (reader.Read())
            //         {
            //             grandTotal = ParseBaseInfo(reader);
            //         }
            //     }
            //
            //     var outputParams = db.GetOuputParams();
            //     totalRows = outputParams["totalrows"] is DBNull ? 0 : Convert.ToInt16(outputParams["totalrows"]);
            // }

            // return new DetailReportResult
            // {
            //     SubTotals = subTotals.ToList(),
            //     Mains = mains.ToList(),
            //     GrandTotal = grandTotal,
            //     Total = total,
            //     SystemParlayOutright = parlayOutright,
            //     TotalItems = totalRows
            // };
            return new DetailReportResult
            {
                SubTotals = dbResult.Item1.ToList(),
                Mains = dbResult.Item2.ToList(),
                Total = dbResult.Item3.First(),
                SystemParlayOutright = dbResult.Item4.ToList(),
                GrandTotal = dbResult.Item5.First(),
                TotalItems = criteria.TotalRows
            };
        }

        public IEnumerable<SportModel> GetSports(SportsCriteria criteria)
        {
            // var sports = new List<SportModel>();
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var entity = new SportModel()
            //             {
            //                 SportType = reader.ToInt("sporttype"),
            //                 RefId = reader.ToString("refid"),
            //                 ResourceId = reader.ToInt("resourceid"),
            //             };
            //             sports.Add(entity);
            //         }
            //     }
            // }
            //
            // return sports;
            var dbResult = repository.Fetch<SportModel>(criteria);
            return dbResult;
        }

        public IEnumerable<LeagueModel> GetLeagues(LeaguesCriteria criteria)
        {
            // var leagues = new List<LeagueModel>();
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var entity = new LeagueModel()
            //             {
            //                 LeagueId = reader.ToInt("leagueid"),
            //                 LeagueName = reader.ToString("leaguename"),
            //             };
            //             leagues.Add(entity);
            //         }
            //     }
            // }
            //
            // return leagues;
            var dbResult = repository.Fetch<LeagueModel>(criteria);
            return dbResult;
        }

        public IEnumerable<CurrencyModel> GetCurrencies(CurrencyCriteria criteria)
        {
            // var currencies = new List<CurrencyModel>();
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var entity = new CurrencyModel()
            //             {
            //                 CurrencyId = reader.ToInt("currencyid"),
            //                 CurrencyName = reader.ToString("currencyname"),
            //             };
            //             currencies.Add(entity);
            //         }
            //     }
            // }
            //
            // return currencies;
            return repository.Fetch<CurrencyModel>(criteria);
        }

        public IEnumerable<BetTypeModel> GetBetTypes(BetTypeCriteria criteria)
        {
            // var betTypes = new List<BetTypeModel>();
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var entity = new BetTypeModel()
            //             {
            //                 BetTypeId = reader.ToInt("bettype"),
            //                 BetTypeName = reader.ToString("bettypename"),
            //             };
            //             betTypes.Add(entity);
            //         }
            //     }
            // }
            //
            // return betTypes;
            return repository.Fetch<BetTypeModel>(criteria);
        }

        public IEnumerable<MatchModel> GetMatches(MatchCriteria criteria)
        {
            // var matches = new List<MatchModel>();
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             var entity = new MatchModel()
            //             {
            //                 MatchId = reader.ToInt("matchid"),
            //                 HomeId = reader.ToInt("homeid"),
            //                 AwayId = reader.ToInt("awayid"),
            //             };
            //             matches.Add(entity);
            //         }
            //     }
            // }
            //
            // return matches;
            return repository.Fetch<MatchModel>(criteria);
        }

        // private ReportRawEntity ParseBaseInfo(IDataReader reader)
        // {
        //     return new ReportRawEntity
        //     {
        //         StakeAvg = reader.ToDecimal("stakeavg"),
        //         BetCount = reader.ToInt("betcount"),
        //         Turnover = reader.ToDecimal("turnover"),
        //         NetTurnover = reader.ToDecimal("netturnover"),
        //         GrossComm = reader.ToDecimal("grosscomm"),
        //         MemberWinLoss = reader.ToDecimal("memberwinloss"),
        //         MemberComm = reader.ToDecimal("membercomm"),
        //         MemberTotal = reader.ToDecimal("membertotal"),
        //         AgentWinLoss = reader.ToDecimal("agentwinloss"),
        //         AgentComm = reader.ToDecimal("agentcomm"),
        //         AgentTotal = reader.ToDecimal("agenttotal"),
        //         MasterWinLoss = reader.ToDecimal("masterwinloss"),
        //         MasterComm = reader.ToDecimal("mastercomm"),
        //         MasterTotal = reader.ToDecimal("mastertotal"),
        //         SuperWinLoss = reader.ToDecimal("superwinloss"),
        //         SuperComm = reader.ToDecimal("supercomm"),
        //         SuperTotal = reader.ToDecimal("supertotal"),
        //         MemberMargin = reader.ToDecimal("membermargin"),
        //         CompanyTotal = reader.ToDecimal("companytotal"),
        //     };
        // }
    }
}
namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using Caching;
    using DataAccess.Impl.Report;
    using DataAccess.Interface.Report;
    using DTO.Customer;
    using DTO.Report;
    using Interface.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Utilities;

#pragma warning disable S109 // Magic numbers should not be used
    public class ReportService : IReportService
    {
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";

        private static readonly ICache Cache = CacheService.CreateInstance(CacheType.MemoryCache);

        private readonly IReportDao _reportDao;

        public ReportService(IReportDao reportRespository = null)
        {
            _reportDao = reportRespository ?? new ReportDao();
        }

        public static DateTime BalanceUpDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);

        public static DateTime MoveBetsDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);

        public DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
            => _reportDao.EAS_CustomerSelUserInfo(groupname, custId, userName, isGetUplineInfo);

        public LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
            => _reportDao.GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);

        public List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
            => _reportDao.EAS_CustomerSelUserSearch(custId, username, groupName);

        public List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_Com(winlostDate, custId, groupName)
                : _reportDao.EAS_Rpt_WinLossDaily_Com14(winlostDate, custId, groupName);

        public List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_SAg(winlostDate, custId, masterId)
                : _reportDao.EAS_Rpt_WinLossDaily_SAg14(winlostDate, custId, masterId);

        public List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_MAg(winlostDate, custId, agentId)
                : _reportDao.EAS_Rpt_WinLossDaily_MAg14(winlostDate, custId, agentId);

        public List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_Ag(winlostDate, custId, memberId)
                : _reportDao.EAS_Rpt_WinLossDaily_Ag14(winlostDate, custId, memberId);

        public List<WinLossDetail> GetSuperWinLossDetail(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetail_Com(groupname, custId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetail> GetMasterWinLossDetail(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetail_SAg(custId, masterId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailMAg> GetAgentWinLossDetail(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetail_MAg(custId, agentId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailAg> GetMemberWinLossDetail(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetail_Ag(custId, memberId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailIBCBetLiveCasino> GetSuperWinLossDetailIBCBetLiveCasino(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(groupname, custId, productIds, fromDate, toDate, BalanceUpDate);

        public List<WinLossDetailIBCBetLiveCasino> GetMasterWinLossDetailIBCBetLiveCasino(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetailIBCBetLiveCasino_SAg(custId, masterId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailIBCBetLiveCasinoMAg> GetAgentWinLossDetailIBCBetLiveCasino(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetailIBCBetLiveCasino_MAg(custId, agentId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailIBCBetLiveCasinoAg> GetMemberWinLossDetailIBCBetLiveCasino(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds)
            => _reportDao.EAS_Rpt_WinLossDetailIBCBetLiveCasino_Ag(custId, memberId, fromDate, toDate, productIds, BalanceUpDate);

        public ArrayList GetSortingOrderList(string groupname)
            => _reportDao.EAS_Rpt_WinLossDaily_SortingOrder(groupname);

        public static DataTable GetBalanceUpDay()
        {
            var balanceUpDay = Cache.Get<DataTable>(BalanceUpDayKey);
            if (balanceUpDay == null)
            {
                var reportDao = new ReportDao();
                balanceUpDay = reportDao.Age_SelBalanceUpDay();
                if (balanceUpDay != null)
                {
                    Cache.Set(BalanceUpDayKey, balanceUpDay, DateTime.Now.AddMinutes(5));
                }
            }

            return balanceUpDay;
        }

        private static bool CheckConstraintMoveBetsDay(DateTime fromDate)
            => DateTime.Compare(fromDate, MoveBetsDate) > 0;
    }
#pragma warning restore S109 // Magic numbers should not be used
}
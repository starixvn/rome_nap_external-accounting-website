namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Base;
    using Caching;
    using DataAccess.Impl.Report;
    using DataAccess.Interface.Report;
    using DTO.Report;
    using Interface.Report;
    using Utilities;

    public class ReportService : ServiceBase, IReportService
    {
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";
        private static readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IReportDao _reportDao;

        public ReportService(IReportDao reportDAO = null)
        {
            _reportDao = reportDAO ?? new ReportDAO();
        }

        public static DateTime BalanceUpDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);

        public static DateTime MoveBetsDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);

        public DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            if (!string.IsNullOrEmpty(userName) || custId != 0)
            {
                try
                {
                    return _reportDao.EAS_CustomerSelUserInfo(groupname, custId, userName, isGetUplineInfo);
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
            }

            return null;
        }

        public List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName) =>
            _reportDao.EAS_CustomerSelUserSearch(custId, username, groupName);

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


        public List<WinLossDetail> GetWinLossDetail(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds) =>
            _reportDao.EAS_Rpt_WinLossDetail_Com(groupname, custId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetail> GetWinLossDetailSuper(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds) =>
            _reportDao.EAS_Rpt_WinLossDetail_SAg(custId, masterId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailMAg> GetWinLossDetailMAg(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds) =>
            _reportDao.EAS_Rpt_WinLossDetail_MAg(custId, agentId, fromDate, toDate, productIds, BalanceUpDate);

        public List<WinLossDetailAg> GetWinLossDetailAg(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds) =>
            _reportDao.EAS_Rpt_WinLossDetail_Ag(custId, memberId, fromDate, toDate, productIds, BalanceUpDate);

        public ArrayList GetSortingOrderList(string groupname) =>
            _reportDao.EAS_Rpt_WinLossDaily_SortingOrder(groupname);

        public static DataTable GetBalanceUpDay()
        {
            var oldBalanceUpDay = _cache.Get<DataTable>(BalanceUpDayKey);

            if (oldBalanceUpDay == null)
            {
                var reportDao = new ReportDAO();

                DataTable balanceUpDay = null;

                try
                {
                    balanceUpDay = reportDao.Age_SelBalanceUpDay();
                }
                catch (SqlException ex)
                {
                    Logs.Error(ex);
                }
                catch (Exception ex)
                {
                    Logs.Error(ex);
                }

                if (balanceUpDay != null)
                {
                    _cache.Set(BalanceUpDayKey, balanceUpDay, DateTime.Now.AddMinutes(5));
                }

                return balanceUpDay;
            }

            return oldBalanceUpDay;
        }

        private static bool CheckConstraintMoveBetsDay(DateTime fromDate)
            => DateTime.Compare(fromDate, MoveBetsDate) > 0;
    }
}
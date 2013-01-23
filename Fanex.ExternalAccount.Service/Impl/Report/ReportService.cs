// -----------------------------------------------------------------------
// <copyright file="ReportService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportService : ServiceBase, IReportService
    {
        private const string BALANCEUP_DAY_KEY = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";
        private readonly IReportDAO reportDAO = null;

        public ReportService(IReportDAO reportDAO = null)
        {
            this.reportDAO = reportDAO ?? new ReportDAO();
        }

        public static DataTable GetBalanceUpDay()
        {
            ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);

            object oldData = cache.Get<DataTable>(BALANCEUP_DAY_KEY);
            if (oldData == null)
            {
                ReportDAO dao = new ReportDAO();
                DataTable dt = dao.Age_SelBalanceUpDay();

                // Store in cache
                cache.Set<DataTable>(BALANCEUP_DAY_KEY, dt, DateTime.Now.AddMinutes(5));

                return dt;
            }

            return oldData as DataTable;
        }

        public DateTime BalanceUpDate
        {
            get
            {
                return Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);
            }
        }

        public DateTime MoveBetsDate
        {
            get
            {
                return Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);
            }
        }

        private bool CheckConstraintMoveBetsDay(DateTime fromDate)
        {
            return DateTime.Compare(fromDate, MoveBetsDate) > 0;
        }

        public System.Data.DataTable GetCustomerInfo(string groupname, int custId, string userName, Boolean isGetUplineInfo)
        {
            if (!string.IsNullOrEmpty(userName) || custId != 0)
            {
                try
                {
                    return reportDAO.EAS_CustomerSelUserInfo(groupname, custId, userName, isGetUplineInfo);
                }
                catch (Exception ex)
                {
                    base.ProcessException(ex);
                }
            }

            return null;
        }

        public List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
        {
            return reportDAO.EAS_CustomerSelUserSearch(custId, username, groupName);
        }

        #region IReportService DailyWinLoss

        public List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
        {
            List<DailyWinLoss> WinLoss = null;

            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_Com(winlostDate, custId, groupName);
            }
            else
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_Com14(winlostDate, custId, groupName);
            }

            return WinLoss;
        }

        public List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId)
        {
            List<DailyWinLoss> WinLoss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_SAg(winlostDate, custId, masterId);
            }
            else
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_SAg14(winlostDate, custId, masterId);
            }

            return WinLoss;
        }

        public List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId)
        {
            List<DailyWinLoss> WinLoss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_MAg(winlostDate, custId, agentId);
            }
            else
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_MAg14(winlostDate, custId, agentId);
            }

            return WinLoss;
        }

        public List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
        {
            List<DailyWinLoss> WinLoss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_Ag(winlostDate, custId, memberId);
            }
            else
            {
                WinLoss = reportDAO.EAS_Rpt_WinLossDaily_Ag14(winlostDate, custId, memberId);
            }

            return WinLoss;
        }

        #endregion IReportService DailyWinLoss

        #region IReportService WinLossDetail

        public List<WinLossDetail> GetWinLossDetail(string groupname, int custId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return reportDAO.EAS_Rpt_WinLossDetail_Com(groupname, custId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetail> GetWinLossDetailSuper(int custId, int masterId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return reportDAO.EAS_Rpt_WinLossDetail_SAg(custId, masterId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetailMAg> GetWinLossDetailMAg(int custId, int agentId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return reportDAO.EAS_Rpt_WinLossDetail_MAg(custId, agentId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetailAg> GetWinLossDetailAg(int custId, int memberId, DateTime fromDate, DateTime toDate, ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return reportDAO.EAS_Rpt_WinLossDetail_Ag(custId, memberId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public ArrayList GetSortingOrderList(string groupname)
        {
            return reportDAO.EAS_Rpt_WinLossDaily_SortingOrder(groupname);
        }
        #endregion IReportService WinLossDetail
    }
}

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
    using System.Data.SqlClient;
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
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";
        private static ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache); 
        private readonly IReportDAO _reportDAO = null;

        public ReportService(IReportDAO reportDAO = null)
        {
            this._reportDAO = reportDAO ?? new ReportDAO();
        }

        public static DateTime BalanceUpDate
        {
            get
            {
                return Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);
            }
        }

        public static DateTime MoveBetsDate
        {
            get
            {
                return Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);
            }
        }

        /// <summary>
        /// Gets balance up day.
        /// </summary>
        /// <returns>DataTable object.</returns>
        public static DataTable GetBalanceUpDay()
        {
            DataTable oldBalanceUpDay = ReportService._cache.Get<DataTable>(BalanceUpDayKey);

            if (oldBalanceUpDay == null)
            {
                ReportDAO reportDao = new ReportDAO();

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
                    // Store in cache
                    ReportService._cache.Set<DataTable>(ReportService.BalanceUpDayKey, balanceUpDay, DateTime.Now.AddMinutes(5));
                }
                                
                return balanceUpDay;
            }

            return oldBalanceUpDay;
        }

        /// <summary>
        /// Gets customer info.
        /// </summary>
        /// <param name="groupname">Group name.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="userName">Customer username.</param>
        /// <param name="isGetUplineInfo">Is get upper info.</param>
        /// <returns>DataTable object.</returns>
        public System.Data.DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            if (!string.IsNullOrEmpty(userName) || custId != 0)
            {
                try
                {
                    return _reportDAO.EAS_CustomerSelUserInfo(groupname, custId, userName, isGetUplineInfo);
                }
                catch (Exception ex)
                {
                    this.ProcessException(ex);
                }
            }

            return null;
        }

        public List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
        {
            return _reportDAO.EAS_CustomerSelUserSearch(custId, username, groupName);
        }

        #region IReportService DailyWinLoss

        /// <summary>
        /// Gets daily win loss report.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
        {
            List<DailyWinLoss> winloss = null;

            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_Com(winlostDate, custId, groupName);
            }
            else
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_Com14(winlostDate, custId, groupName);
            }

            return winloss;
        }

        /// <summary>
        /// Gets daily win loss super.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="masterId">Master id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId)
        {
            List<DailyWinLoss> winloss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_SAg(winlostDate, custId, masterId);
            }
            else
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_SAg14(winlostDate, custId, masterId);
            }

            return winloss;
        }

        /// <summary>
        /// Gets daily win loss master.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId)
        {
            List<DailyWinLoss> winloss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_MAg(winlostDate, custId, agentId);
            }
            else
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_MAg14(winlostDate, custId, agentId);
            }

            return winloss;
        }

        /// <summary>
        /// Gets daily win loss agent.
        /// </summary>
        /// <param name="winlostDate">Win loss date.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="memberId">Member id.</param>
        /// <returns>List object.</returns>
        public List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
        {
            List<DailyWinLoss> winloss = null;
            if (this.CheckConstraintMoveBetsDay(winlostDate))
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_Ag(winlostDate, custId, memberId);
            }
            else
            {
                winloss = _reportDAO.EAS_Rpt_WinLossDaily_Ag14(winlostDate, custId, memberId);
            }

            return winloss;
        }

        #endregion IReportService DailyWinLoss

        #region IReportService WinLossDetail

        public List<WinLossDetail> GetWinLossDetail(
            string groupname, 
            int custId, 
            DateTime fromDate,
            DateTime toDate,
            ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return _reportDAO.EAS_Rpt_WinLossDetail_Com(groupname, custId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetail> GetWinLossDetailSuper(
            int custId, 
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return _reportDAO.EAS_Rpt_WinLossDetail_SAg(custId, masterId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetailMAg> GetWinLossDetailMAg(
            int custId,
            int agentId, 
            DateTime fromDate,
            DateTime toDate,
            ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return _reportDAO.EAS_Rpt_WinLossDetail_MAg(custId, agentId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public List<WinLossDetailAg> GetWinLossDetailAg(
            int custId, 
            int memberId,
            DateTime fromDate,
            DateTime toDate, 
            ICollection<KeyValuePair<string, bool>> filterParams)
        {
            return _reportDAO.EAS_Rpt_WinLossDetail_Ag(custId, memberId, fromDate, toDate, filterParams, BalanceUpDate);
        }

        public ArrayList GetSortingOrderList(string groupname)
        {
            return _reportDAO.EAS_Rpt_WinLossDaily_SortingOrder(groupname);
        }
        #endregion IReportService WinLossDetail

        private bool CheckConstraintMoveBetsDay(DateTime fromDate)
        {
            return DateTime.Compare(fromDate, MoveBetsDate) > 0;
        }
    }
}

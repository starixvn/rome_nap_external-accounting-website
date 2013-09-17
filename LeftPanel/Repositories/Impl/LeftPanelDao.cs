// --------------------------------------------------------------------------
// <copyright file="LeftPanelDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl
{
    using System.Collections;
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Service.Impl.Report;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelDao : ILeftPanelDao
    {
        /// <summary>
        /// Gets customer info.
        /// </summary>
        /// <param name="username">Customer username.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>DataSet object.</returns>
        public DataSet GetCustomerInfo(string username, string groupName = "ACCDDD")
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);

            if (customerInfo != null && customerInfo.Rows.Count != 0)
            {
                var ds = new DataSet();
                var param = new Hashtable();
                param.Add("username", username);

                ds = DbHelper.ExecuteDataSet("Acc_CustomerSelAllInfo", param);

                return ds;
            }

            return null;
        }

        /// <summary>
        /// Update given credit.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>Result object.</returns>
        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdCredit"))
            {
                var param = new Hashtable();
                param.Add("userid", uplineId);
                param.Add("custid", id);
                param.Add("newcredit", newGivenCredit);
                param.Add("ip", CurrentContext.Instance.Ip4Address);
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Gets down-line list.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <returns>DBResult object.</returns>
        public DbResult GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            DbResult rs = null;
            var param = new Hashtable();

            param.Add("custid", custid);
            param.Add("pagesize", pageSize);
            param.Add("pageindex", pageIndex);
            param.Add("totalpages", 0); // 0: Paging

            rs = DbHelper.ExecuteDbResult("Acc_CustomerSelList", param);

            return rs;
        }

        public DataRowCollection GetOnlineList(int custid)
        {
            DbResult rs = null;
            var param = new Hashtable();
            param.Add("custid", custid);
            rs = DbHelper.ExecuteDbResult("Acc_CustomerSelOnline", param);

            return rs.ResultSet.Tables[0].Rows;
        }
    }
}
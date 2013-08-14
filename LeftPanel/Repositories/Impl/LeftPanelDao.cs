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
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Report;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelDao : ILeftPanelDao
    {
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
        
        public Result Update(int id, int uplineId, CustomerModel model)
        {
            throw new System.NotImplementedException();
        }


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

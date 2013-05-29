// --------------------------------------------------------------------------
// <copyright file="ReportRequestTicketRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Fanex.Data;
using Fanex.Puffin.Report.Criteria.ReportRequestTicket;
using Fanex.Puffin.Report.Repositories.Interface;

namespace Fanex.Puffin.Report.Repositories.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ReportRequestTicketRepository : IReportRequestTicketRepository
    {
        public void GetMaxRequestNo(GetMaxRequestNoCriteria criteria, ref Hashtable outputParams)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AddOutParameter("RequestNo", DbType.String, 1000);
                objDb.ExecuteNonQuery();
                outputParams = objDb.GetOuputParams();
            }
        }

        public void AddNewRequestTicket(AddNewRequestTicketCriteria criteria, ref Hashtable outputParams)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AddInParameter("Requestor", DbType.String, criteria.Requestor);
                objDb.AddInParameter("ReqDate", DbType.DateTime, criteria.ReqDate);
                objDb.AddInParameter("ReportName", DbType.String, criteria.ReportName);
                objDb.AddInParameter("Remark", DbType.String, criteria.Remark);
                objDb.AddInParameter("Dateline", DbType.DateTime, criteria.Dateline);
                objDb.AddInParameter("Assignee", DbType.String, criteria.Assignee);
                objDb.AddInParameter("Status", DbType.String, criteria.Status);
                objDb.AddInParameter("RemarkByPreparer", DbType.String, criteria.RemarkByPreparer);
                //objDb.AddOutParameter("ErrorCode", DbType.Decimal, 8);
                //objDb.AddInParameter("ErrorMessage", DbType.String, 1000);
                objDb.ExecuteNonQuery();
                outputParams = objDb.GetOuputParams();
            }
        }

        public void DeleteRequestTicket(DeleteRequestTicketCriteria criteria, ref Hashtable outputParams)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AddInParameter("ReqNo", DbType.String, criteria.ReqNo);
                objDb.AddInParameter("LastModifiedBy", DbType.String, criteria.LastModifiedBy);
                //objDb.AddOutParameter("ErrorCode", DbType.Decimal, 8);
                //objDb.AddInParameter("ErrorMessage", DbType.String, 1000);
                objDb.ExecuteNonQuery();
                outputParams = objDb.GetOuputParams();
            }
        }

        public void UpdateRequestTicket(UpdateRequestTicketCriteria criteria, ref Hashtable outputParams)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AddInParameter("ReqNo", DbType.String, criteria.ReqNo);
                objDb.AddInParameter("LastModifiedBy", DbType.String, criteria.LastModifiedBy);

                objDb.AddInParameter("Remark", DbType.String, criteria.Remark);
                objDb.AddInParameter("Dateline", DbType.DateTime, criteria.Dateline);
                objDb.AddInParameter("Assignee", DbType.String, criteria.Assignee);
                objDb.AddInParameter("Status", DbType.String, criteria.Status);
                objDb.AddInParameter("RemarkByPreparer", DbType.String, criteria.RemarkByPreparer);
                objDb.AddInParameter("ReportName", DbType.String, criteria.ReportName);
                //objDb.AddInParameter("Requestor", DbType.String, criteria.Requestor);

                //objDb.AddOutParameter("ErrorCode", DbType.Decimal, 8);
                //objDb.AddInParameter("ErrorMessage", DbType.String, 1000);
                objDb.ExecuteNonQuery();
                outputParams = objDb.GetOuputParams();
            }
        }
    }
}

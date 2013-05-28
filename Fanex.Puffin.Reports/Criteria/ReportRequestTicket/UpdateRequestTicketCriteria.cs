// --------------------------------------------------------------------------
// <copyright file="UpdateRequestTicketCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System.Diagnostics.CodeAnalysis;
using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Report.Criteria.ReportRequestTicket
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class UpdateRequestTicketCriteria : CriteriaBase
    {
        public string ReqNo { get; set; }

        public string Requestor { get; set; }

        public string DateOfRequest { get; set; }

        public string ReportName { get; set; }

        public string Remark { get; set; }

        public string Dateline { get; set; }

        public string Assignee { get; set; }

        public string Status { get; set; }

        public string RemarkByPreparer { get; set; }

        public string LastModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Report_RequestTicketReport_UpdateRequestTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

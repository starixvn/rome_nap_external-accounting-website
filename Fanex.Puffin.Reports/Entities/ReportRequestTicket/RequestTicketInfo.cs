// --------------------------------------------------------------------------
// <copyright file="RequestTicketReportModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Report.Entities.ReportRequestTicket
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class RequestTicketInfo
    {
        public string RequestNo { get; set; }

        public string Requestor { get; set; }

        public string DateOfRequest { get; set; }

        public string ReportName { get; set; }

        public string Remark { get; set; }

        public string Dateline { get; set; }

        public string Assignee { get; set; }

        public string StatusOfReport { get; set; }

        public string RemarkByPreparer { get; set; }

        public string LastModifier { get; set; }

        public List<RequestTicketInfo> ChildInfo;

        #region DTOField


        #endregion DTOField
    }
}

// --------------------------------------------------------------------------
// <copyright file="GetMaxRequestNoCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetMaxRequestNoCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Report_RequestTicketReport_GetMaxRequestNo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

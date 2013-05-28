// --------------------------------------------------------------------------
// <copyright file="IReportRequestTicketRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Fanex.Puffin.Report.Criteria.ReportRequestTicket;

namespace Fanex.Puffin.Report.Repositories.Interface
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public interface IReportRequestTicketRepository
    {
        void GetMaxRequestNo(GetMaxRequestNoCriteria criteria, ref Hashtable outputParams);

        void AddNewRequestTicket(AddNewRequestTicketCriteria criteria, ref Hashtable outputParams);

        void DeleteRequestTicket(DeleteRequestTicketCriteria criteria, ref Hashtable outputParams);

        void UpdateRequestTicket(UpdateRequestTicketCriteria criteria, ref Hashtable outputParams);
    }
}

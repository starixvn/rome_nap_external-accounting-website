// -----------------------------------------------------------------------
// <copyright file="ISingleTickketService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Models.SingleTicket;

namespace Fanex.Puffin.Admin.Services.Interface
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public interface ISingleTickketService
    {
        List<Bettype> SelBettype(GetBettypeCriteria criteria);
    }
}

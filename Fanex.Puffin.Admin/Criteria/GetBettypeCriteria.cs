// -----------------------------------------------------------------------
// <copyright file="GetBettypeCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using Fanex.NAP.Framework.Data;
using System.Diagnostics.CodeAnalysis;
using Fanex.Puffin.Admin.Common;

namespace Fanex.Puffin.Admin.Criteria
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class GetBettypeCriteria : CriteriaBase
    {

        public Enums.TicketControlMode Mode { get; set; }

        public override string GetSettingKey()
        {
            return "Common_SelBettype";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

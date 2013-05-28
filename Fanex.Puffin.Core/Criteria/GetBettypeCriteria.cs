// -----------------------------------------------------------------------
// <copyright file="GetBettypeCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.NAP.Framework.Data;
using System.Diagnostics.CodeAnalysis;
using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Criteria
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class GetBettypeCriteria : CriteriaBase
    {
        public ProductType Mode { get; set; }

        public override string GetSettingKey()
        {
            return "Core_SelBettype";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

// -----------------------------------------------------------------------
// <copyright file="GetAllLicenseeCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.NAP.Framework.Data;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Core.Criteria
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class GetAllLicenseeCriteria : CriteriaBase
    {
        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name</returns>
        public override string GetSettingKey()
        {
            return "Misc_Acc_SiteSel_Licensee";
        }

        /// <summary>
        /// Check data's constrains
        /// </summary>
        /// <returns>True: valid; False: invalid</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}

// -----------------------------------------------------------------------
// <copyright file="GetCurrencyCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCurrencyCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Core_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

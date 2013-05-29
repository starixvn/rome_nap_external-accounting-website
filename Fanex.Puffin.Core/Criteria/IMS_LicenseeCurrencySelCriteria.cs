// -----------------------------------------------------------------------
// <copyright file="IMS_LicenseeCurrencySelCriteria.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.4.23">Created</change>
// </history>

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    public class IMS_LicenseeCurrencySelCriteria : CriteriaBase
    {
        public int LicenseeId { get; set; }

        public override string GetSettingKey()
        {
            return "IMS_LicenseeCurrencySel";
        }

        public override bool IsValid() { return true; }
    }
}
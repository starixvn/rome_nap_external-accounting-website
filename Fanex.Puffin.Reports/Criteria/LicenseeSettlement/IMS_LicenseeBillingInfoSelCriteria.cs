// -----------------------------------------------------------------------
// <copyright file="IMS_LicenseeBillingInfoSelCriteria.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.4.23">Created</change>
// </history>

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Report.Criteria.LicenseeSettlement
{
    public class IMS_LicenseeBillingInfoSelCriteria : CriteriaBase
    {
        public int LicenseeId { get; set; }
        public int ProductId { get; set; }

        public override string GetSettingKey()
        {
            return "IMS_LicenseeBillingInfoSel";
        }

        public override bool IsValid() { return true; }
    }
}
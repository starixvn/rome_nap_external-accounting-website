// -----------------------------------------------------------------------
// <copyright file="IMS_Rpt_LicenseeSettlementCriteria.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.4.23">Created</change>
// </history>

using System;
using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Report.Criteria.LicenseeSettlement
{
    [Serializable]
    public class IMS_Rpt_LicenseeSettlementCriteria : CriteriaBase
    {
        public int LicenseeId { get; set; }
        public string LicenseeName { get; set; }
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Product { get; set; }
        public string ProductName { get; set; }
        public int Currency { get; set; }
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; }

        [SpParam(Ignored = true)]
        public int Month { get; set; }
        [SpParam(Ignored = true)]
        public int Year { get; set; }

        public IMS_Rpt_LicenseeSettlementCriteria()
            : base()
        {
            this.Month = DateTime.Now.Month - 1;
            this.Year = DateTime.Now.Year;
        }

        public override string GetSettingKey()
        {
            return "IMS_Rpt_LicenseeSettlement";
        }

        public override bool IsValid() { return true; }
    }
}
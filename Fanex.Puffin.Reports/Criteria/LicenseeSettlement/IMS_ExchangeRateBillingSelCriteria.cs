// -----------------------------------------------------------------------
// <copyright file="IMS_ExchangeRateBillingSelCriteria.cs" company="Nexcel">
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
    public class IMS_ExchangeRateBillingSelCriteria : CriteriaBase
    {
        public string Currency { get; set; }
        public DateTime ExchangeDate { get; set; }
        public int BaseCurrencyId { get; set; }
        public string BaseCurrencyName { get; set; }

        public override string GetSettingKey()
        {
            return "IMS_ExchangeRateBillingSel";
        }

        public override bool IsValid() { return true; }
    }
}
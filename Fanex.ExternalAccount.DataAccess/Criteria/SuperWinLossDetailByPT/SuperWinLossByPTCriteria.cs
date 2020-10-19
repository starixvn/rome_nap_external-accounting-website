namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DataAccess.Extensions;
    using Fanex.NAP.Framework.Data;
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTCriteria : CriteriaBase
    {
        public SuperWinLossDetailByPTCriteria(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay,
            string productIds,
            //UserLevel custLevel,
            string productDetails)
        {
            //CustLevel = custLevel;
            CustId = custId;
            FromDate = fromDate.Date;
            ToDate = toDate.Date;
            BalanceUpDay = balanceUpDay;
            MoveBetsDay = moveBetsDay;
            ProductDetails = productDetails;
            ProductIds = productIds;
        }

        //[SpParam(Ignored = true)]
        //public UserLevel CustLevel { get; }

        public int CustId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetsDay { get; }

        public string ProductIds { get; }

        public string ProductDetails { get; }

        public override string GetSettingKey() => "Age_Rpt_WinLossByProduct_Master";

        //public override string GetSettingKey()
        //    => CustLevel.IsAgentLevel()
        //    ? "Age_Rpt_WinLossByProduct_Member"
        //    : CustLevel.IsMasterLevel()
        //    ? "Age_Rpt_WinLossByProduct_Agent"
        //    : "Age_Rpt_WinLossByProduct_Master";
    }
}

using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetSuperDailyWinlossCriteria : CriteriaBase
    {
        public GetSuperDailyWinlossCriteria(DateTime WinlossDate, int CustId, int MasterId, DateTime MoveBetsDate)
        {
            this.WinlossDate = WinlossDate;
            this.CustId = CustId;
            this.MasterId = MasterId;
            this.MoveBetsDate = MoveBetsDate;
        }

        public DateTime WinlossDate { get; }

        public int CustId { get; }

        public int MasterId { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinlossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_SAg" : "EAS_Rpt_WinLossDaily_SAg14";
    }
}

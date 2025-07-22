using Fanex.ExternalAccount.Service.Interface.Report;
using System;

namespace Fanex.ExternalAccount.Controllers.Requests
{
    public class DrillDownCashFeeRequest : IDrillDownCashFeeRequest
    {
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}

using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Report
{
    public class CashFeeGroup
    {
        public string GroupName { get; set; }

        public List<CashFeeItem> FeeItems { get; set; } = new List<CashFeeItem>();

        public CashFeeItem Total { get; set; }
    }
}

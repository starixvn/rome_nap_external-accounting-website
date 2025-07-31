using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Report
{
    public class CashFee
    {
        public List<CashFeeGroup> FeeGroups { get; set; } = new List<CashFeeGroup>();

        public CashFeeItem GrandTotal { get; set; }
    }
}

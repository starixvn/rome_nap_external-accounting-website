namespace Fanex.ExternalAccount.RewardManagement.Models
{
    using System.Collections.Generic;

    public class RewardReport
    {
        public IEnumerable<RewardRecord> Items { get; set; }
        public int TotalItems { get; set; }
    }
}
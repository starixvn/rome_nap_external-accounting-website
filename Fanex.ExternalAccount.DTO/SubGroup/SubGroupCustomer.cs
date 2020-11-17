namespace Fanex.ExternalAccount.DTO.SubGroup
{
    using System.Collections.Generic;

    public class SubGroupCustomer
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public bool IsClosed { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsEnabled { get; set; }

        public string SubGroup { get; set; }

        public IEnumerable<SubGroupStatus> SubGroupStatusList { get; set; }
    }
}

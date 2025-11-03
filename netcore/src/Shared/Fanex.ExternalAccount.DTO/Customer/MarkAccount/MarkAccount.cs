using System;
using Fanex.ExternalAccount.Utilities;

namespace Fanex.ExternalAccount.DTO.Customer.MarkAccount
{
    public class MarkAccount
    {
        public string Username { get; set; }

        public int CustId { get; set; }

        public int Level { get; set; }

        public int TypeId { get; set; }

        public DateTime MarkedDate { get; set; }

        public string MarkDateTime => Utils.ToStringExt(MarkedDate);

        public string Remark { get; set; }

        public int AgentId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class UpdateMultipleCustomerCommand
    {
        public string ProductGroups { get; set; }

        public string CustomerIds { get; set; }

        public string CustomerNames { get; set; }

        public int CustId { get; set; }

        public int RoleId { get; set; }

    }
}

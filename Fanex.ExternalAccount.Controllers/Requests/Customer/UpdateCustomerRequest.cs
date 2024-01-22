using Fanex.Customer.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class UpdateCustomerRequest
    {
        public GeneralInfo GeneralInfo { get; set; }

        public string ProductGroups { get; set; }

        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int UplineRoleId { get; set; }
    }
}

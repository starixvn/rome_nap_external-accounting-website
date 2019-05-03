using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.ViewModel.Customer.Suspend
{
    public class MultipleUpdateSuspendRequest
    {
        public int CustId { get; set; }

        public string UserName { get; set; }

        public bool Suspended { get; set; }
    }
}

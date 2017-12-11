namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PTVirtualSportsUpdateModel
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int TargetCustId { get; set; }

        public int TargetRoleId { get; set; }

        public int UpdateType { get; set; }

        public bool IsMulti { get; set; }
    }
}

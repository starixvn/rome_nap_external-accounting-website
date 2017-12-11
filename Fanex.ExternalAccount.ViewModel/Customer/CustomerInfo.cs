namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CustomerInfo
    {
        public CustomerInfo() 
        {
        }

        public int CustomerId { get; set; }

        public bool DisableLiveCasino { get; set; }

        public int ExChangeId { get; set; }

        public string Fax { get; set; }

        public string FirstName { get; set; }

        public bool IsSyncLiveCasino { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public string NickName { get; set; }

        public string Phone { get; set; }

        public byte RoleId { get; set; }

        public string Site { get; set; }

        public bool UplineDisableLiveCasino { get; set; }

        public string UserName { get; set; }

        public string GroupName { get; set; }
    }
}
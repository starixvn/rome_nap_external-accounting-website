using System.Collections.Generic;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public class CustomerStatusDTO
    {
        public bool DisableTransfer { get; set; }

        public bool HasSecurityCode { get; set; }

        public bool UplineHasSecurityCode { get; set; }

        public List<Product> ProductGroups { get; set; }

        public CustomerStatusDTO()
        {
            ProductGroups = new List<Product>();
        }
    }
}
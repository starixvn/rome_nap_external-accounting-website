using System.Collections.Generic;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class CustomerSettingsViewModel
    {
        public CustomerSettingsViewModel(IEnumerable<Product> productGroups, IEnumerable<Product> products)
        {
            ProductGroups = productGroups;
            Products = products;
        }

        public IEnumerable<Product> ProductGroups { get; }

        public IEnumerable<Product> Products { get; }
    }
}
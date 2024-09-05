using System.Collections.Generic;
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class CustomerSettingsViewModel
    {
        public CustomerSettingsViewModel(IEnumerable<Product> productGroups)
        {
            ProductGroups = productGroups;
        }

        public IEnumerable<Product> ProductGroups { get; }
    }
}
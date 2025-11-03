namespace Fanex.ExternalAccount.ViewModel.Customer.OpenCustomer
{
    using DTO.Customer.Status;
    using Requests.Customer.OpenCustomer;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class MultipleOpenCustomerViewModel
    {
        private readonly GetMultipleOpenCustomerInfoQuery query;
        private readonly IEnumerable<CustomerClosedStatusRow> customerClosedStatusList;

        public MultipleOpenCustomerViewModel(GetMultipleOpenCustomerInfoQuery query, IEnumerable<CustomerClosedStatusRow> customerClosedStatusList)
        {
            this.query = query;
            this.customerClosedStatusList = customerClosedStatusList;
        }

        public int RoleId => query.RoleId;

        public ReadOnlyCollection<CustomerClosedStatusRow> CustomerClosedStatusList()
        {
            return customerClosedStatusList?.ToList()?.AsReadOnly();
        }
    }
}
namespace Fanex.ExternalAccount.ViewModel.Customer.Suspend
{
    using Fanex.ExternalAccount.DTO.Customer.Status;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class MultipleUpdateViewModel
    {
        private readonly IEnumerable<CustomerSuspendStatusRow> _customerSuspendStatusList;

        public MultipleUpdateViewModel(int roleId, IEnumerable<CustomerSuspendStatusRow> customerSuspendStatusList)
        {
            RoleId = roleId;
            _customerSuspendStatusList = customerSuspendStatusList;
        }

        public int RoleId { get; }

        public ReadOnlyCollection<CustomerSuspendStatusRow> CustomerSuspendStatusList()
        {
            return _customerSuspendStatusList?.ToList()?.AsReadOnly();
        }
    }
}

namespace Fanex.ExternalAccount.ViewModel.Customer.CustomerList
{
    public class CustomerListViewModel
    {
        public int? CustId { get; set; }

        public int? RoleId { get; set; }

        public bool HasReadProtectCustomerFunction { get; set; }

        public bool IsMultipleCurrencyGroup { get; set; }
    }
}

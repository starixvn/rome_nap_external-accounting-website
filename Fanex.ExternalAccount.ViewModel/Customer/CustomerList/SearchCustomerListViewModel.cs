namespace Fanex.ExternalAccount.Controllers.ViewModels.CustomerList
{
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using System.Collections.Generic;

    public class SearchCustomerListViewModel
    {
        public List<CustomerListDTO> CustomerList { get; set; }

        public int PageIndex { get; set; }

        public int TotalRows { get; set; }

        public SearchCustomerListViewModel()
        {
            CustomerList = new List<CustomerListDTO>();
        }
    }
}

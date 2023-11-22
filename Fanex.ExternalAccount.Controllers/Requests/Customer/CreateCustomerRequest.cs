using Fanex.ExternalAccount.Controllers.ViewModels.Customer;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class CreateCustomerRequest
    {
        public GeneralInfo GeneralInfo { get; set; }

        public string ProductGroups { get; set; }
    }
}
using Fanex.Customer.Client.Models;

namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class CreateCustomerCommand
    {
        public GeneralInfo GeneralInfo { get; set; }

        public string Group { get; set; }

        public string ProductGroups { get; set; }
    }
}
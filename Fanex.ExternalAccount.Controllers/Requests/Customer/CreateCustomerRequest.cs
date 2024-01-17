namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class CreateCustomerRequest
    {
        public int UplineId { get; set; }

        public byte UplineRoleId { get; set; }

        public string UplineUsername { get; set; }
    }
}
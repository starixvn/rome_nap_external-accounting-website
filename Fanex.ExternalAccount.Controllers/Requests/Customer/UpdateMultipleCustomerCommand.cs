namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class UpdateMultipleCustomerCommand
    {
        public string ProductGroups { get; set; }

        public string CustomerIds { get; set; }

        public string CustomerNames { get; set; }

        public int CustId { get; set; }

        public int RoleId { get; set; }

        public int UplineRoleId { get; set; }

        public string Site { get; set; }

        public string SiteLabel { get; set; }

        public string GroupName { get; set; }
    }
}
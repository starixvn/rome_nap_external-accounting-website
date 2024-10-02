namespace Fanex.ExternalAccount.Controllers.Requests.Customer
{
    public class GetCustomerSettingRequest
    {
        public int CustId { get; set; }

        public int ProductGroupId { get; set; } = 1;

        public bool IsGetUpLine { get; set; } = false;

        public byte RoleId { get; set; }

        public byte UplineRoleId { get; set; } = 0;

        public byte SystemId { get; set; }

        public string Language { get; set; } = "en-US";

        public string SiteLabel { get; set; } = "";

        public bool IsMultipleMode { get; set; } = false;

        public string GroupName { get; set; } = "";

        public bool IsCopy { get; set; } = false;
    }
}

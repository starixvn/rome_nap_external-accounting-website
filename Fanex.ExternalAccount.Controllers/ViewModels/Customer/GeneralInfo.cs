namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class GeneralInfo
    {
        public string Username { get; set; }

        public int UpLineRoleId { get; set; }

        public string Password { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public decimal MaxCredit { get; set; }

        public decimal MemberMaxCredit { get; set; }

        public string Fax { get; set; }

        public bool DisableAutoMPT { get; set; }

        public string TransferOption { get; set; }

        public bool IsAutomateTransfer { get; set; }

        public string Site { get; set; }

        public string SiteLabel { get; set; }

        public int Currency { get; set; }

        public string Group { get; set; }

        public int UplineCustId { get; set; }
    }
}
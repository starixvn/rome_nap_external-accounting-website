namespace Fanex.ExternalAccount.SearchAndCopy.Requests
{
    public class CloneSettingRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public decimal Credit { get; set; }

        public string ExistingUsername { get; set; }

        public string GroupName { get; set; }
    }
}

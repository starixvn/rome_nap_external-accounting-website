namespace Fanex.ExternalAccount.DTO.UserControl
{
    public class User
    {
        public int UserID { get; set; }

        public int GroupID { get; set; }

        public bool Enabled { get; set; }

        public string Creator { get; set; }

        public string CreatedDate { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string GroupName { get; set; }

        public string Password { get; set; }
    }
}
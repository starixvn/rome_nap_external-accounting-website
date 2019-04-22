namespace Fanex.ExternalAccount.DTO.UserControl
{
    public class UserPolicy : GroupPolicy
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public bool Enabled { get; set; }
    }
}

namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    public class NewMarkAccountRequest
    {
        public string GroupName { get; set; }

        public int TypeId { get; set; }

        public string Usernames { get; set; }

        public string Remark { get; set; }
    }
}

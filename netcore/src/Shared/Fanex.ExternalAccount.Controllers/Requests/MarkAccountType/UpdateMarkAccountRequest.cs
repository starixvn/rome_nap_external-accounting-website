namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    public class UpdateMarkAccountRequest
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int TypeId { get; set; }

        public string Remark { get; set; }
    }
}

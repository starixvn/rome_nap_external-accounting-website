namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    public class SearchMarkAccountRequest
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }

        public string UserName { get; set; }

        public string Level { get; set; }

        public string Type { get; set; }

        public int? PageSize { get; set; }

        public int? PageIndex { get; set; }
    }
}

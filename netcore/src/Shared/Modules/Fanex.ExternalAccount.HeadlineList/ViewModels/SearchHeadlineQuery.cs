namespace Fanex.ExternalAccount.HeadlineList.ViewModels
{
    public class SearchHeadlineQuery
    {
        public string Username { get; set; }
        
        public string GroupHeadline { get; set; }
        
        public int Status { get; set; }
        
        public int Page { get; set; } = 1;
        
        public int PageSize { get; set; } = 10;
    }
}
namespace Fanex.ExternalAccount.HeadlineList.Models
{
    using System.Collections.Generic;

    public class SearchHeadlineResult
    {
        public int TotalGroups { get; set; }
        
        public int TotalHeadlines { get; set; }
        
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
        
        public IList<GroupHeadline> GroupHeadlines { get; set; }
    }
}
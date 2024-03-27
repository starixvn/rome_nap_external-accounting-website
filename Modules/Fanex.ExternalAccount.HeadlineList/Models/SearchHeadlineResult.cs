using System.Collections.Generic;

namespace Fanex.ExternalAccount.HeadlineList.Models
{
    public class SearchHeadlineResult
    {
        public int TotalGroups { get; set; }
        public int PageIndex { get; set; }
        
        public IList<GroupHeadline> GroupHeadlines { get; set; }
    }
}
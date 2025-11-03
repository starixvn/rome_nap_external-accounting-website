namespace Fanex.ExternalAccount.HeadlineList.Models
{
    using System.Collections.Generic;
    using System.Linq;
    
    public class GroupsHeadline
    {
        public IEnumerable<GroupHeadline> Groups { get; set; }
        
        public int TotalHeadlineGroups => Groups.Count();
        
        public int TotalHeadlines { get; set; }
    }
}
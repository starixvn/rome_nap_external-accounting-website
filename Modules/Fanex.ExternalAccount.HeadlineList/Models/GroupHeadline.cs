using System.Collections.Generic;
using System.Linq;

namespace Fanex.ExternalAccount.HeadlineList.Models
{
    public class GroupHeadline
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        
        public IList<HeadlineEntity> HeadlineEntities { get; set; }
    }
    
    public class GetGroupHeadlinesResult
    {
        public IEnumerable<GroupHeadline> GroupHeadlines { get; set; }

        public int TotalHeadlineGroups => GroupHeadlines.Count();
        public int TotalHeadlines { get; set; }
    }
}
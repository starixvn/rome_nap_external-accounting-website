using System.Collections.Generic;

namespace Fanex.ExternalAccount.HeadlineList.Models
{
    public class GroupHeadline
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        
        public IList<HeadlineEntity> HeadlineEntities { get; set; }
    }
}
namespace Fanex.ExternalAccount.HeadlineList.Models
{
    using System.Collections.Generic;
    
    public class GroupHeadline
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        
        public IList<HeadlineEntity> HeadlineEntities { get; set; }
        
        
    }
    
}
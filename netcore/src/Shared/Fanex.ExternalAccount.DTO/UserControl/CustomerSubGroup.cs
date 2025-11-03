namespace Fanex.ExternalAccount.DTO.UserControl
{
    using System.Collections.Generic;

    public class CustomerSubGroup
    {
        public string Username { get; set; }

        public string Status { get; set; }

        public IEnumerable<SubGroup> SubGroups { get; set; }
    }
}

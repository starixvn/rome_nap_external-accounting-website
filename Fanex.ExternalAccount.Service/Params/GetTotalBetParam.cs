using System.Collections.Generic;

namespace Fanex.ExternalAccount.Service.Params
{
    public class GetTotalBetParam
    {
        public string GroupName { get; set; }
        public int ViewMode { get; set; }
        public int League { get; set; }
        public IEnumerable<int> Events { get; set; }
    }
}

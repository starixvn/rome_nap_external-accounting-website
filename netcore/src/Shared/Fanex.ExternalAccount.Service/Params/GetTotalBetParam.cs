using System.Collections.Generic;

namespace Fanex.ExternalAccount.Service.Params
{
    public class GetTotalBetParam
    {
        public string GroupName { get; set; }
        public int ViewMode { get; set; }
        public IEnumerable<int> Leagues { get; set; }
        public IEnumerable<int> Events { get; set; }
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; }
    }
}

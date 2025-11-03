namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System.Collections.Generic;
    using Core.Context;

    public class TotalBetRequest
    {
        private readonly IExtRequest extRequest;

        public TotalBetRequest() : this(null) { }

        public TotalBetRequest(IExtRequest extRequest)
        {
            this.extRequest = extRequest ?? new ExtRequest();
        }

        public string GroupName => extRequest.GetGroupName();
        public int ViewMode { get; set; }
        public IEnumerable<int> Leagues { get; set; }
        public IEnumerable<int> Events { get; set; }
    }

    public class TotalBetMCRequest : TotalBetRequest
    {
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; }
    }
}

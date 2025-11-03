namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Fanex.ExternalAccount.Core.Context;
    using System;
    
    public class GetExchangeRateRequest
    {
        private readonly IExtRequest extRequest;
        private readonly CurrentSession currentSession;

        public GetExchangeRateRequest() : this(null, null) { }

        public GetExchangeRateRequest(IExtRequest extRequest, CurrentSession currentSession)
        {
            this.extRequest = extRequest ?? new ExtRequest();
            this.currentSession = currentSession ?? new CurrentSession();
        }

        public string MainGroupName => currentSession.ParentGroupName;
        public string GroupName => extRequest.GetGroupName();
        public int GroupId => extRequest.GetGroupId();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WinLossDetailRequest
    {
        private readonly IExtRequest extRequest;
        private readonly CurrentSession currentSession;

        public WinLossDetailRequest() : this(null, null)
        {
        }

        public WinLossDetailRequest(IExtRequest extRequest, CurrentSession currentSession)
        {
            this.extRequest = extRequest ?? new ExtRequest();
            this.currentSession = currentSession ?? new CurrentSession();
        }

        public string MainGroupName => currentSession.ParentGroupName;
        public int GroupId => extRequest.GetGroupId();
        public string GroupName => extRequest.GetGroupName();
        public string Username { get; set; }
        public int Srecommend { get; set; }
        public int Mrecommend { get; set; }
        public int Recommend { get; set; }
        public int CustId { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public int Viewlevel { get; set; }
        public string ProductIds { get; set; }
        public string ResourceIds { get; set; }
        public int UserId { get; set; }

        public bool IsSearch() => CustId == 0;
    }
}

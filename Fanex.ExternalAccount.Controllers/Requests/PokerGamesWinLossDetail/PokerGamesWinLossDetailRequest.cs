namespace Fanex.ExternalAccount.Controllers.Requests.PokerGamesWinLossDetail
{
    using System;
    using Core.Context;

    public class PokerGamesWinLossDetailRequest
    {
        private readonly IExtRequest extRequest;
        public PokerGamesWinLossDetailRequest() : this(null)
        {
        }

        public PokerGamesWinLossDetailRequest(IExtRequest extRequest)
        {
            this.extRequest = extRequest ?? new ExtRequest();
        }

        public string groupName => extRequest.GetGroupName();

        public string username { get; set; }
        public int srecommend { get; set; }
        public int mrecommend { get; set; }
        public int recommend { get; set; }
        public int custId { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public int viewlevel { get; set; }

        public bool IsSearch() => custId == 0;
    }
}

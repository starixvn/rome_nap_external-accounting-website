namespace Fanex.ExternalAccount.Controllers.Requests.PokerGamesWinLossDetail
{
    using System;
    using Service.Interface.Report;

    public class SearchPokerGamesWinLossDetailRequest : ISearchPokerGamesWinLossDetailRequest
    {
        public SearchPokerGamesWinLossDetailRequest(PokerGamesWinLossDetailRequest request)
        {
            GroupName = request.groupName;
            Username = request.username;
            FromDate = request.fromdate;
            ToDate = request.todate;
        }

        public string GroupName { get; }
        public string Username { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
    }
}

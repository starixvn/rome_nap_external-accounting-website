namespace Fanex.ExternalAccount.Controllers.Requests.PokerGamesWinLossDetail
{
    using Core.Shared;

    public class ExportPokerGamesWinLossDetailRequest : PokerGamesWinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}PokerGamesWinLossDetail_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}

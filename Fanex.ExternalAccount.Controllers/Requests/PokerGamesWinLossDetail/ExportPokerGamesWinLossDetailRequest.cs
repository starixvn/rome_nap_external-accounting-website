namespace Fanex.ExternalAccount.Controllers.Requests.PokerGamesWinLossDetail
{
    using Core.Shared;

    public class ExportPokerGamesWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}PokerGamesWinLoss_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}

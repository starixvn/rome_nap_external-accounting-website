namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface PokerGamesWinLossDetailService
    {
        dynamic DrillDownPokerGamesWinLossDetail(IDrillDownPokerGamesWinLossDetailRequest request);

        dynamic SearchPokerGamesWinLossDetail(ISearchPokerGamesWinLossDetailRequest request);
    }
}
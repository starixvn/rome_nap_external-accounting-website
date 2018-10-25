namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface PokerGamesWinLossDetailService
    {
        dynamic DrillDownPokerGamesWinLossDetail(IDrillDownWinLossDetailRequest request);

        dynamic SearchPokerGamesWinLossDetail(ISearchWinLossDetailRequest request);
    }
}
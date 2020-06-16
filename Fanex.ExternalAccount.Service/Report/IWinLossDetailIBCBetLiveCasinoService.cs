namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface IWinLossDetailIBCBetLiveCasinoService
    {
        dynamic DrillDownWinLossDetailIBCBetLiveCasino(IDrillDownWinLossDetailIBCBetLiveCasinoRequest request);

        dynamic SearchWinLossDetailIBCBetLiveCasino(ISearchWinLossDetailIBCBetLiveCasinoRequest request);
    }
}

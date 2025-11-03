namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface WinLossDetailService
    {
        dynamic DrillDownWinLossDetail(IDrillDownWinLossDetailRequest request);

        dynamic DrillDownWinLossMCDetail(IDrillDownWinLossDetailRequest request);

        dynamic SearchWinLossDetail(ISearchWinLossDetailRequest request);

        dynamic GetExtraPT(IViewExtraPTRequest request);

        dynamic DrillDownCashFee(IDrillDownCashFeeRequest request);
    }
}
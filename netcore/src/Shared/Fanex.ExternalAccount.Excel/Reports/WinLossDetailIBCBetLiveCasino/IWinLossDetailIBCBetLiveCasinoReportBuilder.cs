namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using System.IO;

    public interface IWinLossDetailIBCBetLiveCasinoReportBuilder
    {
        bool IsBold { get; }
        string FromDate { get; }
        string ToDate { get; }

        MemoryStream Build(string products = null);
    }
}
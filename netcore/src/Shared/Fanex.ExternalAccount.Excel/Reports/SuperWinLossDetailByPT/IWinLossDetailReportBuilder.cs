namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using System.IO;

    public interface ISuperWinLossDetailByPTReportBuilder
    {
        bool IsBold { get; }
        string FromDate { get; }
        string ToDate { get; }
        MemoryStream Build(string products = null);
    }
}
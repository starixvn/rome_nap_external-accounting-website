namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.IO;

    public interface ICashFeeBuilder
    {
        bool IsBold { get; }
        string FromDate { get; }
        string ToDate { get; }
        string UserName { get; }
        MemoryStream Build(string products = null, string userName = null);
    }
}
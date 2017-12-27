namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.IO;

    public interface IWinLossDetailReportBuilder
    {

        bool IsBold { get; }
        int RoleId { get; }
        string FromDate { get; }
        string ToDate { get; }
        MemoryStream Build(string products = null);
    }
}
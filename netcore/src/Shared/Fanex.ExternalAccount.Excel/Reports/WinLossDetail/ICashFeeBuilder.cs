namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.IO;

    public interface IWinLossDetailReportBuilder
    {

        bool IsBold { get; }
        int RoleId { get; }
        int UplineRoleId { get; }
        string FromDate { get; }
        string ToDate { get; }
        MemoryStream Build(string products = null);
    }
}
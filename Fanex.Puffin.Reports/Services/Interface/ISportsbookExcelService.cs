namespace Fanex.Puffin.Report.Services.Interface
{
    public interface ISportsbookExcelService
    {
        void ExportReportToExcel(string exchange, string product, string fileName, bool isSaveFile = false);
    }
}
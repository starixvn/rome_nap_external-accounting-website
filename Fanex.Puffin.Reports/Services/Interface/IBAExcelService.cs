namespace Fanex.Puffin.Report.Services.Interface
{
    public interface IBAExcelService
    {
        void ExportReportToExcel(string exchange, string product);
    }
}
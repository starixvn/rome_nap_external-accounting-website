namespace Fanex.ExternalAccount.Excel
{
    using NPOI.HSSF.UserModel;

    public interface IReportBuilder
    {
        HSSFWorkbook Build();
    }
}
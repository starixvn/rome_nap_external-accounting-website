using NPOI.HSSF.UserModel;

namespace Fanex.ExternalAccount.Excel
{
    public interface IReportBuilder
    {
        HSSFWorkbook Build();
    }
}
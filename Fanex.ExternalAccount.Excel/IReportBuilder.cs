/*
 * Created 20120216@Leo
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Excel
{
    using NPOI.HSSF.UserModel;

    public interface IReportBuilder
    {
        HSSFWorkbook Build();
    }
}
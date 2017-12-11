namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    public static class RowExtension
    {
        public static ICell CreateCell(this IRow row, int colId, ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(colId);
            cell.CellStyle = cellStyle;
            return cell;
        }

        public static void WriteCells(this IRow row, int colId, params object[] values)
        {
            int id = colId;
            if (values.Length == 1 && values[0] is List<object>)
            {
                values = ((List<object>)values[0]).ToArray();
            }

            foreach (var value in values)
            {
                row.GetCell(id).SetCellValue(value);
                id++;
            }
        }
    }
}
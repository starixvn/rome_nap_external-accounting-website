namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    public static class RowExtension
    {
        public static ICell CreateCell(this IRow row, int colId, ICellStyle cellStyle)
        {
            var cell = row.CreateCell(colId);
            cell.CellStyle = cellStyle;
            return cell;
        }

        public static void WriteCells(this IRow row, int colId, params object[] values)
        {
            var id = colId;
            var newValue = values;
            if (newValue.Length == 1 && newValue[0] is List<object>)
            {
                newValue = ((List<object>)newValue[0]).ToArray();
            }

            foreach (var value in newValue)
            {
                row.GetCell(id).SetCellValue(value);
                id++;
            }
        }
    }
}
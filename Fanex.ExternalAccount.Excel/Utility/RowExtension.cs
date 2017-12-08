namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RowExtension
    {
        public static ICell CreateCell(this IRow row, int colId, ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(colId);
            cell.CellStyle = cellStyle;
            return cell;
        }

        /// <summary>
        /// Writes cell values.
        /// </summary>
        /// <param name="row">Row to write values.</param>
        /// <param name="colId">Column id.</param>
        /// <param name="values">Values to write.</param>
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
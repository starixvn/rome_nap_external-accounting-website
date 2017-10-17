// -----------------------------------------------------------------------
// <copyright file="HSSFSheetExtension.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class HSSFSheetExtension
    {
        /// <summary>
        /// Copies rows.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="sourceSheet">Worksheet source.</param>
        /// <param name="sourceRowNum">Row number.</param>
        /// <param name="destinateRowNum">Destination row number.</param>
        /// <param name="total">Total rows.</param>
        public static void CopyRows(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum, int total)
        {
            if (total <= 0)
            {
                return;
            }

            for (var i = 0; i < total; i++)
            {
                sheet.CopyRow(sourceSheet, sourceRowNum + i, destinateRowNum + i);
            }
        }

        /// <summary>
        /// Copies rows.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="sourceSheet">Worksheet source.</param>
        /// <param name="sourceRowNum">Row number.</param>
        /// <param name="destinateRowNum">Destination row number.</param>
        /// <param name="firstColumn">First column index.</param>
        /// <param name="lastColumn">Last column index.</param>
        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum, int firstColumn, int lastColumn)
        {
            if (sheet.LastRowNum < destinateRowNum)
            {
                for (var i = sheet.LastRowNum + 1; i <= destinateRowNum; i++)
                {
                    sheet.CreateRow(i);
                }
            }

            var sourceRow = sourceSheet.GetRow(sourceRowNum);
            if (sourceRow == null)
            {
                return;
            }

            var newRow = sheet.GetRow(destinateRowNum);
            newRow = newRow ?? sheet.CreateRow(destinateRowNum);
            newRow.Height = sourceRow.Height;
            int firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumn);
            int lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumn + 1);
            for (int i = firstCellNum; i < lastCellNum; i++)
            {
                var oldCell = sourceRow.GetCell(i);
                if (oldCell == null)
                {
                    continue;
                }

                var newCell = newRow.CreateCell(i);
                CloneCell(newCell, oldCell);
                CellRangeAddress sourceRange = null;
                for (var j = 0; j < sourceSheet.NumMergedRegions; j++)
                {
                    CellRangeAddress range = sourceSheet.GetMergedRegion(j);
                    if (range.FirstColumn == oldCell.ColumnIndex && range.FirstRow == oldCell.RowIndex)
                    {
                        sourceRange = range;
                        break;
                    }
                }

                if (sourceRange != null)
                {
                    int lastRow = newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow;
                    int firstCol = newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn;
                    var newRange = new CellRangeAddress(newCell.RowIndex, lastRow, newCell.ColumnIndex, firstCol);
                    sheet.AddMergedRegion(newRange);
                }
            }
        }

        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum)
        {
            sheet.CopyRow(sourceSheet, sourceRowNum, destinateRowNum, 0, int.MaxValue - 1);
        }

        public static void RemoveRow(this ISheet sheet, int rowIndex)
        {
            sheet.RemoveRow(sheet.GetRow(rowIndex));
        }

        public static void RemoveColumns(this ISheet sheet, List<int> columns)
        {
            columns.Sort();
            for (int i = columns.Count - 1; i >= 0; i--)
            {
                RemoveColumn(sheet, columns[i]);
            }
        }

        /// <summary>
        /// Remove column from worksheet.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="columnIndex">Column index.</param>
        public static void RemoveColumn(this ISheet sheet, int columnIndex)
        {
            int maxColumn = 0;
            bool adjustColumn = true;
            for (int r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);
                int lastColumn = row.LastCellNum;
                maxColumn = Math.Max(maxColumn, lastColumn);
                if (lastColumn < columnIndex)
                {
                    continue;
                }

                SetColumnWidth(ref sheet, ref columnIndex, ref lastColumn, ref row, ref adjustColumn);

                MergeColumn(ref sheet, ref columnIndex, ref r);

                adjustColumn = false;
            }
        }

        public static void CloneCell(this ISheet sheet, ICell newCell, ICell oldCell)
        {
            CloneCell(newCell, oldCell);
        }

        public static ICell GetCell(this ISheet sheet, int rowIndex, int colIndex)
        {
            return sheet.GetRow(rowIndex).GetCell(colIndex);
        }

        public static int MergeCells(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            var rangeAddress = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            return sheet.AddMergedRegion(rangeAddress);
        }

        /// <summary>
        /// Removes merge regions.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="firstRow">First row index.</param>
        /// <param name="firstCol">First column index.</param>
        /// <param name="lastRow">Last row index.</param>
        /// <param name="lastCol">Last column index.</param>
        public static void RemoveMergedRegions(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                var mergedRegion = sheet.GetMergedRegion(i);
                if (mergedRegion.FirstRow >= firstRow && mergedRegion.LastRow <= lastRow &&
                    mergedRegion.FirstColumn >= firstCol && mergedRegion.LastColumn <= lastCol)
                {
                    sheet.RemoveMergedRegion(i);
                }
            }
        }

        /// <summary>
        /// Creates a cell value.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="rowId">Row id of worksheet.</param>
        /// <param name="colId">Column id of worksheet.</param>
        /// <param name="rowSpan">Row span of column.</param>
        /// <param name="colSpan">Column span of row.</param>
        /// <returns>ICell object.</returns>
        public static ICell CreateCell(this ISheet sheet, int rowId, int colId, int rowSpan, int colSpan)
        {
            Debug.Assert(rowSpan >= 0, "rowSpan must be more than or equal 0");
            Debug.Assert(colSpan >= 0, "colSpan must be more than or equal 0");

            ICell cell = sheet.CreateCell(rowId, colId);
            if (rowSpan == 0 && colSpan == 0)
            {
                return cell;
            }

            MergeCells(sheet, rowId, colId, rowId + rowSpan, colId + colSpan);
            return cell;
        }

        public static ICell CreateCell(this ISheet sheet, int rowId, int colId, int rowSpan, int colSpan, ICellStyle cellStyle)
        {
            ICell cell = sheet.CreateCell(rowId, colId, rowSpan, colSpan);
            cell.CellStyle = cellStyle;
            return cell;
        }

        /// <summary>
        /// Creates a cell values.
        /// </summary>
        /// <param name="sheet">Worksheet object.</param>
        /// <param name="rowId">Row id of worksheet.</param>
        /// <param name="colId">Column id of worksheet.</param>
        /// <returns>ICell object.</returns>
        public static ICell CreateCell(this ISheet sheet, int rowId, int colId)
        {
            IRow row = sheet.GetRow(rowId);
            if (row == null)
            {
                row = sheet.CreateRow(rowId);
            }

            ICell cell = row.GetCell(colId);
            if (cell == null)
            {
                cell = row.CreateCell(colId);
            }

            return cell;
        }

        /// <summary>
        /// Clones a cell.
        /// </summary>
        /// <param name="newCell">New cell value.</param>
        /// <param name="oldCell">Old cell value.</param>
        private static void CloneCell(ICell newCell, ICell oldCell)
        {
            if (oldCell.CellComment != null)
            {
                newCell.CellComment = oldCell.CellComment;
            }

            if (oldCell.Hyperlink != null)
            {
                newCell.Hyperlink = oldCell.Hyperlink;
            }

            if (oldCell.CellStyle != null)
            {
                newCell.CellStyle = oldCell.CellStyle;
            }

            newCell.SetCellType(oldCell.CellType);

            switch (newCell.CellType)
            {
                case CellType.Boolean:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;

                case CellType.Numeric:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;

                case CellType.String:
                    newCell.SetCellValue(oldCell.StringCellValue);
                    break;

                case CellType.Error:
                    newCell.SetCellValue(oldCell.ErrorCellValue);
                    break;

                case CellType.Formula:
                    newCell.CellFormula = oldCell.CellFormula;
                    break;
            }
        }

        /// <summary>
        /// Sets column width.
        /// </summary>
        /// <param name="sheet">ISheet object.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="lastColumn">Last column index.</param>
        /// <param name="row">IRow object.</param>
        /// <param name="adjustColumn">Is adjusted column constant.</param>
        private static void SetColumnWidth(ref ISheet sheet, ref int columnIndex, ref int lastColumn, ref IRow row, ref bool adjustColumn)
        {
            for (int x = columnIndex; x < lastColumn; x++)
            {
                var oldCell = row.GetCell(x);
                if (oldCell != null)
                {
                    row.RemoveCell(oldCell);
                }

                if (x < lastColumn)
                {
                    var nextCell = row.GetCell(x + 1);
                    if (nextCell != null)
                    {
                        var newCell = row.CreateCell(x, nextCell.CellType);
                        if (adjustColumn)
                        {
                            sheet.SetColumnWidth(newCell.ColumnIndex, sheet.GetColumnWidth(nextCell.ColumnIndex));
                        }

                        CloneCell(newCell, nextCell);
                    }
                }
                else
                {
                    var removeCell = row.GetCell(x);
                    if (removeCell != null)
                    {
                        row.RemoveCell(removeCell);
                    }
                }
            }
        }

        /// <summary>
        /// Merges columns.
        /// </summary>
        /// <param name="sheet">ISheet object.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="rowIndex">Row index.</param>
        private static void MergeColumn(ref ISheet sheet, ref int columnIndex, ref int rowIndex)
        {
            for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                var mergedRegion = sheet.GetMergedRegion(i);
                if (rowIndex == mergedRegion.FirstRow)
                {
                    if (columnIndex == mergedRegion.FirstColumn && columnIndex == mergedRegion.LastColumn)
                    {
                        sheet.RemoveMergedRegion(i);
                    }
                    else if (mergedRegion.FirstColumn > columnIndex)
                    {
                        mergedRegion.FirstColumn = mergedRegion.FirstColumn - 1;
                        mergedRegion.LastColumn = mergedRegion.LastColumn - 1;
                    }
                    else if (columnIndex <= mergedRegion.LastColumn)
                    {
                        mergedRegion.LastColumn = mergedRegion.LastColumn - 1;
                    }
                }
            }
        }
    }
}
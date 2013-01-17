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

        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum, int firstColumn, int lastColumn)
        {
            if (sheet.LastRowNum < destinateRowNum)
            {
                for (var i = sheet.LastRowNum + 1; i <= destinateRowNum; i++)
                {
                    sheet.CreateRow(i);
                }
            }

            // Get the source / new row
            var sourceRow = sourceSheet.GetRow(sourceRowNum);
            if (sourceRow == null)
            {
                return;
            }

            var newRow = sheet.GetRow(destinateRowNum);
            if (newRow == null)
            {
                newRow = sheet.CreateRow(destinateRowNum);
            }

            newRow.Height = sourceRow.Height;

            // Loop through source columns to add to new row
            int firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumn);
            int lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumn + 1);
            for (int i = firstCellNum; i < lastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                var oldCell = sourceRow.GetCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    continue;
                }

                var newCell = newRow.CreateCell(i);
                CloneCell(newCell, oldCell);

                // copy Merged
                CellRangeAddress sourceRange = null;
                for (var j = 0; j < sourceSheet.NumMergedRegions; j++)
                {
                    CellRangeAddress range = sourceSheet.GetMergedRegion(j);
                    if (range.FirstColumn == oldCell.ColumnIndex
                        && range.FirstRow == oldCell.RowIndex)
                    {
                        sourceRange = range;
                        break;
                    }
                }

                if (sourceRange != null)
                {
                    CellRangeAddress newRange = new CellRangeAddress(newCell.RowIndex, newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow, newCell.ColumnIndex, newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);
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

        public static void RemoveColumn(this ISheet sheet, int columnIndex)
        {
            int maxColumn = 0;
            bool adjustColumn = true;
            for (int r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);

                // if no row exists here; then nothing to do; next!
                if (row == null)
                {
                    continue;
                }

                // if the row doesn't have this many columns then we are good; next!
                int lastColumn = row.LastCellNum;
                maxColumn = Math.Max(maxColumn, lastColumn);

                if (lastColumn < columnIndex)
                {
                    continue;
                }

                for (int x = columnIndex; x < lastColumn; x++)
                {
                    var oldCell = row.GetCell(x);
                    if (oldCell != null)
                    {
                        row.RemoveCell(oldCell);
                    }

                    if (x < lastColumn)
                    {
                        // move cell
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

                // adjust merged region
                for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
                {
                    var mergedRegion = sheet.GetMergedRegion(i);

                    ////if (r >= mergedRegion.FirstRow && r <= mergedRegion.LastRow)
                    if (r == mergedRegion.FirstRow)
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

                adjustColumn = false;
            }

            //// Adjust the column widths
            ////for (int c = 0; c < maxColumn; c++)
            ////{
            ////    sheet.SetColumnWidth(c, sheet.GetColumnWidth(c + 1));
            ////}
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

        /*
         * Takes an existing Cell and merges all the styles and forumla into the new one
         */

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
                case CellType.BOOLEAN:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;

                case CellType.NUMERIC:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;

                case CellType.STRING:
                    newCell.SetCellValue(oldCell.StringCellValue);
                    break;

                case CellType.ERROR:
                    newCell.SetCellValue(oldCell.ErrorCellValue);
                    break;

                case CellType.FORMULA:
                    newCell.CellFormula = oldCell.CellFormula;
                    break;
            }
        }
    }
}
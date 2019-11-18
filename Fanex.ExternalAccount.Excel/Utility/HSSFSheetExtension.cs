namespace Fanex.ExternalAccount.Excel.Utility
{
    using System;
    using System.Collections.Generic;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public static class HSSFSheetExtension
    {
        public static void CopyRows(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum,
            int total)
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

        public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowNum, int destinateRowNum,
            int firstColumn, int lastColumn)
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
            var firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumn);
            var lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumn + 1);
            for (var i = firstCellNum; i < lastCellNum; i++)
            {
                var oldCell = sourceRow.GetCell(i);
                if (oldCell != null)
                {
                    var newCell = newRow.CreateCell(i);
                    CloneCell(newCell, oldCell);

                    var sourceRange = FindSourceRange(sourceSheet, oldCell);
                    if (sourceRange != null)
                    {
                        var lastRow = newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow;
                        var firstCol = newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn;
                        var newRange = new CellRangeAddress(newCell.RowIndex, lastRow, newCell.ColumnIndex, firstCol);
                        sheet.AddMergedRegion(newRange);
                    }
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
            for (var i = columns.Count - 1; i >= 0; i--)
            {
                RemoveColumn(sheet, columns[i]);
            }
        }

        public static void RemoveColumn(this ISheet sheet, int columnIndex)
        {
            var maxColumn = 0;
            var adjustColumn = true;
            for (var r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);
                int lastColumn = row.LastCellNum;
                maxColumn = Math.Max(maxColumn, lastColumn);
                if (lastColumn < columnIndex)
                {
                    continue;
                }

                SetColumnWidth(sheet, columnIndex, lastColumn, row, adjustColumn);

                MergeColumn(sheet, columnIndex, r);

                adjustColumn = false;
            }
        }

        public static ICell GetCell(this ISheet sheet, int rowIndex, int colIndex) => sheet.GetRow(rowIndex).GetCell(colIndex);

        private static CellRangeAddress FindSourceRange(ISheet sourceSheet, ICell oldCell)
        {
#pragma warning disable S3353 // Unchanged local variables should be "const"
            CellRangeAddress sourceRange = null;
#pragma warning restore S3353 // Unchanged local variables should be "const"
            for (var j = 0; j < sourceSheet.NumMergedRegions; j++)
            {
                var range = sourceSheet.GetMergedRegion(j);
                if (range.FirstColumn == oldCell.ColumnIndex && range.FirstRow == oldCell.RowIndex)
                {
                    return range;
                }
            }

            return sourceRange;
        }

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

        private static void SetColumnWidth(ISheet sheet, int columnIndex, int lastColumn, IRow row, bool adjustColumn)
        {
            for (var x = columnIndex; x < lastColumn; x++)
            {
                var oldCell = row.GetCell(x);
                if (oldCell != null)
                {
                    row.RemoveCell(oldCell);
                }

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
        }

        private static void MergeColumn(ISheet sheet, int columnIndex, int rowIndex)
        {
            for (var i = sheet.NumMergedRegions - 1; i >= 0; i--)
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
                        mergedRegion.FirstColumn--;
                        mergedRegion.LastColumn--;
                    }
                    else if (columnIndex <= mergedRegion.LastColumn)
                    {
                        mergedRegion.LastColumn--;
                    }
                }
            }
        }
    }
}
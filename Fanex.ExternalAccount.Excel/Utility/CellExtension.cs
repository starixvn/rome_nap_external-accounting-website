namespace Fanex.ExternalAccount.Excel.Utility
{
    using System;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public static class CellExtension
    {
        public static void SetCellValue(this ICell cell, object value)
        {
            if (value == null)
            {
                cell.SetCellValue("0");
            }
            else
            {
                if (value is string)
                {
                    cell.SetCellValue((string) value);
                }
                else if (value is int)
                {
                    cell.SetCellValue((int) value);
                }
                else if (value is long)
                {
                    cell.SetCellValue((long) value);
                }
                else if (value is double)
                {
                    cell.SetCellValue((double) value);
                }
                else if (value is decimal)
                {
                    SetCellValue(cell, (object) (decimal) value);
                }
                else if (value is bool)
                {
                    cell.SetCellValue((bool) value);
                }
                else if (value is DateTime)
                {
                    cell.SetCellValue((DateTime) value);
                }
                else
                {
                    var newValue = value as IRichTextString;
                    if (newValue != null)
                    {
                        cell.SetCellValue(newValue);
                    }
                    else
                    {
                        cell.SetCellValue("Undefined");
                    }
                }
            }
        }

        public static void SetCellValue(this ICell cell, double? value)
        {
            if (value == null)
            {
                value = 0;
            }

            cell.SetCellValue((double) value);
        }

        public static void SetCellValue(this ICell cell, decimal value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, decimal? value)
        {
            if (value == null)
            {
                value = 0;
            }

            cell.SetCellValue((decimal) value);
        }

        public static void SetCellValue(this ICell cell, int value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, int? value)
        {
            if (value == null)
            {
                value = 0;
            }

            cell.SetCellValue((int) value);
        }

        public static void SetCellValue(this ICell cell, long value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, long? value)
        {
            if (value == null)
            {
                value = 0;
            }

            cell.SetCellValue((long) value);
        }

        public static void CopyFrom(this ICell cell, ICell sourceCell)
        {
            CellRangeAddress sourceRange = null;
            for (var i = 0; i < sourceCell.Sheet.NumMergedRegions; i++)
            {
                var range = sourceCell.Sheet.GetMergedRegion(i);
                if (range.FirstColumn <= sourceCell.ColumnIndex
                    && range.LastColumn >= sourceCell.ColumnIndex
                    && range.FirstRow <= sourceCell.RowIndex
                    && range.LastRow >= sourceCell.RowIndex)
                {
                    sourceRange = range;
                    break;
                }
            }

            if (sourceRange != null)
            {
                cell.Sheet.AddMergedRegion(sourceRange.Clone());
            }

            cell.CellStyle = sourceCell.CellStyle;
        }
    }
}
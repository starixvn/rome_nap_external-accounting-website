// --------------------------------------------------------------------------
// <copyright file="ExcelExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.6">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Web;
using ExcelUtil.Spreadsheet;
using Fanex.Puffin.Core.Resources;

namespace Fanex.Puffin.Core.Extensions
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class ExcelExtensions
    {
        #region Constanst values

        public const int FONT_WIDTH = 30;
        public const int FONT_SIZE = 160;
        private const string FONT_NAME = "Tahoma";
        private const string NEGATIVE_NUMBER = "#,##0{0}_);[RED](#,##0{1})";
        private const string NORMAL_NUMBER_FORMAT = "#,##0{0}_);-#,##0{1}";
        private const string POSITIVE_NUMBER_FORMAT = "[RED][>=0.1]#,##0{0}_);-#,##0{1}";

        public static readonly Color BACKGROUND_COLOR = Color.White;
        public static readonly Color HEADER_TITLE_COLOR = Color.FromArgb(60, 94, 165);
        public static readonly Color EVEN_ROW_COLOR = Color.FromArgb(228, 242, 253);
        public static readonly Color ODD_ROW_COLOR = Color.White;
        public static readonly Color SUB_TOTAL_ROW_COLOR = Color.FromArgb(194, 217, 236);
        public static readonly Color TOTAL_ROW_COLOR = Color.FromArgb(244, 244, 190);
        public static readonly Color GRAND_TOTAL_ROW_COLOR = Color.FromArgb(219, 225, 187);
        public static readonly Color GRAND_TOTAL_SEPERATOR_COLOR = Color.FromArgb(255, 102, 0);
        public static readonly Color REVENUE_LICENSEE_COLOR = Color.FromArgb(213, 132, 52);

        #endregion Constanst values

        #region Response Stream

        /// <summary>
        /// Response Stream for export excel file
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="reportName"></param>
        public static void ReponseStream(this ExcelFile excelFile, string reportName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.xls", reportName));
            excelFile.SaveXls(response.OutputStream);
        }

        #endregion Response Stream

        #region Title Cell Format

        /// <summary>
        /// Format exist Title Cell
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="bgColor"></param>
        public static void FormatTitleCell(this CellStyle cellStyle,
            Color bgColor)
        {
            cellStyle.FormatCellText(bgColor, true);
            cellStyle.Font.Color = Color.White;
            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            cellStyle.WrapText = true;
        }

        /// <summary>
        /// Get Style of Title Cell
        /// </summary>
        /// <param name="bgColor"></param>
        /// <returns></returns>
        public static CellStyle GetCellTitleStyle(Color bgColor)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatTitleCell(bgColor);
            return cellStyle;
        }

        #endregion

        #region Text Cell Format

        /// <summary>
        /// Get Style of Text Cell
        /// </summary>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <returns></returns>
        public static CellStyle GetCellTextStyle(Color bgColor,
            bool isBold)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(bgColor, isBold);
            return cellStyle;
        }

        /// <summary>
        /// Get Cell Style with Full Options
        /// </summary>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="textAlign"></param>
        /// <param name="isItalic"></param>
        /// <param name="fontColor"></param>
        /// <returns></returns>
        public static CellStyle GetCellTextStyle(Color bgColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign,
            bool isItalic,
            Color fontColor)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(bgColor, isBold, textAlign, isItalic, fontColor);
            return cellStyle;
        }
        
        /// <summary>
        /// Get Style of Text Cell
        /// </summary>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="textAlign"></param>
        /// <returns></returns>
        public static CellStyle GetCellTextStyle(Color bgColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellText(bgColor, isBold, textAlign);
            return cellStyle;
        }

        /// <summary>
        /// Format Exist Cell Text
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        public static void FormatCellText(this CellStyle cellStyle,
            Color bgColor,
            bool isBold)
        {
            cellStyle.FormatCellText(bgColor, isBold, HorizontalAlignmentStyle.General);
        }
        
        /// <summary>
        /// Format Text Cell With Background Color, Bold or not, And Text Align
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="textAlign"></param>
        public static void FormatCellText(this CellStyle cellStyle,
            Color bgColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign)
        {
            cellStyle.FormatCellText(bgColor, isBold, textAlign, false, Color.Black);
        }

        /// <summary>
        /// Format Cell Text Full Options
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="textAlign"></param>
        /// <param name="isItalic"></param>
        /// <param name="fontColor"></param>
        public static void FormatCellText(this CellStyle cellStyle,
            Color bgColor,
            bool isBold,
            HorizontalAlignmentStyle textAlign,
            bool isItalic,
            Color fontColor)
        {
            cellStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
            cellStyle.Font.Name = FONT_NAME;
            cellStyle.Font.Size = FONT_SIZE;
            if (bgColor != null)
            {
                cellStyle.FillPattern.SetSolid(bgColor);
            }

            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = textAlign;
            if (isBold)
            {
                cellStyle.Font.Weight = ExcelFont.BoldWeight;
            }

            if (isItalic)
            {
                cellStyle.Font.Italic = true;
            }

            if (fontColor != null)
            {
                cellStyle.Font.Color = fontColor;
            }
        }

        #endregion Text Cell Format

        #region Number Cell Format

        /// <summary>
        /// Get Style Of Cell Number
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="isPossitive"></param>
        /// <param name="isNagative"></param>
        /// <returns></returns>
        public static CellStyle GetCellNumberStyle(
            string symbol,
            Color bgColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellNumberStyle(symbol, bgColor, isBold, isPossitive, isNagative);
            return cellStyle;
        }

        /// <summary>
        /// Get Style Of Cell Number
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bgColor"></param>
        /// <param name="fontColor"></param>
        /// <param name="isBold"></param>
        /// <param name="isPossitive"></param>
        /// <param name="isNagative"></param>
        /// <returns></returns>
        public static CellStyle GetCellNumberStyle(string symbol,
            Color bgColor,
            Color fontColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            CellStyle cellStyle = new CellStyle();
            cellStyle.FormatCellNumberStyle(symbol, bgColor, fontColor, isBold, isPossitive, isNagative);
            return cellStyle;
        }

        /// <summary>
        /// Format Cell Which contains number value
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="symbol"></param>
        /// <param name="bgColor"></param>
        /// <param name="isBold"></param>
        /// <param name="isPossitive"></param>
        /// <param name="isNagative"></param>
        public static void FormatCellNumberStyle(this CellStyle cellStyle,
            string symbol,
            Color bgColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            cellStyle.FormatCellNumberStyle(symbol,
                bgColor,
                Color.Black,
                isBold,
                isPossitive,
                isNagative);
        }

        /// <summary>
        /// Format Cell Which contains number value
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="symbol"></param>
        /// <param name="bgColor"></param>
        /// <param name="fontColor"></param>
        /// <param name="isBold"></param>
        /// <param name="isPossitive"></param>
        /// <param name="isNagative"></param>
        public static void FormatCellNumberStyle(this CellStyle cellStyle,
            string symbol,
            Color bgColor,
            Color fontColor,
            bool isBold,
            bool isPossitive,
            bool isNagative)
        {
            cellStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            cellStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);

            if (isNagative)
            {
                cellStyle.NumberFormat = string.Format(NEGATIVE_NUMBER, symbol, symbol);
            }
            else if (isPossitive)
            {
                cellStyle.NumberFormat = string.Format(POSITIVE_NUMBER_FORMAT, symbol, symbol);
            }
            else
            {
                cellStyle.NumberFormat = string.Format(NORMAL_NUMBER_FORMAT, symbol, symbol);
            }

            cellStyle.Font.Name = FONT_NAME;
            cellStyle.Font.Size = FONT_SIZE;

            if (bgColor != null)
            {
                cellStyle.FillPattern.SetSolid(bgColor);
            }

            if (fontColor != null)
            {
                cellStyle.Font.Color = fontColor;
            }

            if (isBold)
            {
                cellStyle.Font.Weight = ExcelFont.BoldWeight;
            }
        }

        #endregion

        #region Row Format

        /// <summary>
        /// Format Empty Row Extension
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="currentRow"></param>
        /// <param name="endColumn"></param>
        public static void ExportEmptyRow(this ExcelWorksheet ws,
            int startRow,
            int startColumn,
            ref int currentRow,
            int endColumn)
        {
            var currentColumn = startColumn;
            var currentCellRange = ws.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Empty;
            currentCellRange.Style.FormatCellText(Color.White, false);
            currentCellRange.Style.Font.Color = Color.White;
            currentRow++;
        }

        #endregion Row Format
        #region Report Format

        /// <summary>
        /// ExportEmptyReport: export empty report message
        /// Ex: There is no data.
        /// </summary>
        /// <param name="ws">Excel Worksheet</param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="currentRow"></param>
        /// <param name="endColumn"></param>
        public static void ExportEmptyReport(this ExcelWorksheet ws,
            int startRow,
            int startColumn,
            ref int currentRow,
            int endColumn)
        {
            var currentColumn = startColumn;
            var currentCellRange = ws.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.EmptyReportMessage;
            currentCellRange.Style.FormatCellText(Color.White, true);
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentRow++;
        }

        #endregion
    }
}
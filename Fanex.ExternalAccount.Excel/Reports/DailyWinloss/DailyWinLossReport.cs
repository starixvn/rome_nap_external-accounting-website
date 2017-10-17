// --------------------------------------------------------------------------
// <copyright file="DailyWinLossReport.cs" company="">
// Copyright (c) . All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    /// <summary>
    /// Daily win loss report.
    /// </summary>
    internal class DailyWinLossReport : TemplateReport, IReportBuilder
    {
        private readonly DailyWinLossReportBuilder _builder;

        public DailyWinLossReport(string filePath, DailyWinLossReportBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        /// <summary>
        /// Builds workbook.
        /// </summary>
        /// <returns>HSSFWorkbook object.</returns>
        public HSSFWorkbook Build()
        {
            var data = _builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("DailyWinLoss");
            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            int rowId = 0, colId = 0, id = 1;
            var mainTitle = (_builder.RoleId == 5 ? "Super" : _builder.RoleId == 4 ? "Master" : _builder.RoleId == 3 ? "Agent" : "Member") +
                " Daily Win Loss on " + _builder.WinlostDate;
            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);
            rowId = 3;
            if (data.Count > 0)
            {
                WriteCells(ref resultSheet, ref templateSheet, rowId, colId, id);
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return HssfWorkbook;
        }

        /// <summary>
        /// Writes cells of workbook.
        /// </summary>
        /// <param name="resultSheet">ISheet of result data.</param>
        /// <param name="templateSheet">ISheet of template data.</param>
        /// <param name="rowId">Row id of workbook.</param>
        /// <param name="colId">Column id of workbook.</param>
        /// <param name="id">Order id of row.</param>
        private void WriteCells(ref ISheet resultSheet, ref ISheet templateSheet, int rowId, int colId, int id)
        {
            List<object> values;
            int totalColumn = 19, lastColumnIndex = colId + totalColumn - 1;
            var data = _builder.Data;
            foreach (var item in data)
            {
                resultSheet.CopyRow(templateSheet, id % 2 == 0 ? 1 : 3, rowId, colId, lastColumnIndex);

                values = new List<object>
                    {
                        item.No,
                        item.Username,
                        item.TotalTO,
                        item.ComTO,
                        item.ComTOPercent,
                        item.SAgTO,
                        item.SAgTOPercent,
                        item.MAgTO,
                        item.MAgTOPercent,
                        item.AgTO,
                        item.AgTOPercent,
                        item.ComWinlost,
                        item.ComWinlostPercent,
                        item.SAgWinlost,
                        item.SAgWinlostPercent,
                        item.MAgWinlost,
                        item.MAgWinlostPercent,
                        item.AgWinlost,
                        item.AgWinlostPercent
                    };
                resultSheet.GetRow(rowId).WriteCells(colId, values);
                id++;
                rowId++;
            }

            WriteTotal(ref resultSheet, ref templateSheet, rowId, colId, lastColumnIndex);
        }

        /// <summary>
        /// Writes total cells of workbook.
        /// </summary>
        /// <param name="resultSheet">ISheet of result data.</param>
        /// <param name="templateSheet">ISheet of template data.</param>
        /// <param name="rowId">Row id of workbook.</param>
        /// <param name="colId">Column id of workbook.</param>
        /// <param name="lastColumnIndex">Order id of last column.</param>
        private void WriteTotal(ref ISheet resultSheet, ref ISheet templateSheet, int rowId, int colId, int lastColumnIndex)
        {
            var data = _builder.Data;
            resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
            var values = new List<object>
                {
                    string.Empty,
                    "Grand Total",
                    data.Sum(p => p.TotalTO),
                    data.Sum(p => p.ComTO),
                    data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.ComTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    data.Sum(p => p.SAgTO),
                    data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.SAgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    data.Sum(p => p.MAgTO),
                    data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.MAgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    data.Sum(p => p.AgTO),
                    data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.AgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    data.Sum(p => p.ComWinlost),
                    data.Sum(p => p.ComTO) != 0 ? data.Sum(p => p.ComWinlost) / data.Sum(p => p.ComTO) * 100 : 0,
                    data.Sum(p => p.SAgWinlost),
                    data.Sum(p => p.SAgTO) != 0 ? data.Sum(p => p.SAgWinlost) / data.Sum(p => p.SAgTO) * 100 : 0,
                    data.Sum(p => p.MAgWinlost),
                    data.Sum(p => p.MAgTO) != 0 ? data.Sum(p => p.MAgWinlost) / data.Sum(p => p.MAgTO) * 100 : 0,
                    data.Sum(p => p.AgWinlost),
                    data.Sum(p => p.AgTO) != 0 ? data.Sum(p => p.AgWinlost) / data.Sum(p => p.AgTO) * 100 : 0
                };
            resultSheet.GetRow(rowId).WriteCells(colId, values);
        }
    }
}
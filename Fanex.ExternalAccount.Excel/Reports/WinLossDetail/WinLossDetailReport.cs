// --------------------------------------------------------------------------
// <copyright file="WinLossDetailReport.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    /// <summary>
    /// Win loss detail report builder.
    /// </summary>
    internal class WinLossDetailReport : TemplateReport, IReportBuilder
    {
        private WinLossDetailReportBuilder _builder;

        public WinLossDetailReport(string filePath, WinLossDetailReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            this._builder = builder;
        }

        /// <summary>
        /// Builds report.
        /// </summary>
        /// <returns>Workbook object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public HSSFWorkbook Build()
        {
            var data = this._builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("WinLossDetail");

            // apply Bold on all fonts
            if (this._builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            // Header
            string mainTitle = string.Empty;
            if (this._builder.RoleId == 4)
            {
                mainTitle = "Super";
            }
            else if (this._builder.RoleId == 3)
            {
                mainTitle = "Master";
            }

            if (this._builder.FromDate.Equals(this._builder.ToDate))
            {
                mainTitle += " Win Loss Detail on " + this._builder.FromDate;
            }
            else
            {
                mainTitle += " Win Loss Detail From " + this._builder.FromDate + " To " + this._builder.ToDate;
            }

            mainTitle += " - " + this.Products;

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 3;
            int id = 1;

            if (data.Count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 17;
                int lastColumnIndex = colId + totalColumn - 1;

                foreach (var item in data)
                {
                    if (id % 2 == 0)
                    {
                        resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                    }
                    else
                    {
                        resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                    }

                    values = new List<object>
                    {
                        item.Username,
                        item.Turnover,
                        item.NetTurnover,
                        item.GrossComm,
                        item.MemberWinlost,
                        item.PlayerComm,
                        item.MemberTotal,
                        item.AgWinlost,
                        item.AgComm,
                        item.AgTotal,
                        item.MAgWinlost,
                        item.MAgComm,
                        item.MAgTotal,
                        item.SAgWinlost,
                        item.SAgComm,
                        item.SAgTotal,
                        item.Company
                    };

                    resultSheet.GetRow(rowId).WriteCells(colId, values);

                    id++;
                    rowId++;
                }

                // total
                resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);

                values = new List<object>
                {
                    "Grand Total",
                    data.Sum(p => p.Turnover),
                    data.Sum(p => p.NetTurnover),
                    data.Sum(p => p.GrossComm),
                    data.Sum(p => p.MemberWinlost),
                    data.Sum(p => p.PlayerComm),
                    data.Sum(p => p.MemberTotal),
                    data.Sum(p => p.AgWinlost),
                    data.Sum(p => p.AgComm),
                    data.Sum(p => p.AgTotal),
                    data.Sum(p => p.MAgWinlost),
                    data.Sum(p => p.MAgComm),
                    data.Sum(p => p.MAgTotal),
                    data.Sum(p => p.SAgWinlost),
                    data.Sum(p => p.SAgComm),
                    data.Sum(p => p.SAgTotal),
                    data.Sum(p => p.Company)
                };

                resultSheet.GetRow(rowId).WriteCells(colId, values);
            }

            this.HssfWorkbook.RemoveSheetAt(this.HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }
    }
}
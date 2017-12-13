namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using NPOI.HSSF.UserModel;
    using Utilities;
    using Utility;

    internal class StatementPage : TemplateReport, IReportBuilder
    {
        private readonly StatementPageBuilder _builder;

        public StatementPage(string filePath, StatementPageBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public HSSFWorkbook Build()
        {
            var data = _builder.Data;
            if (data == null)
            {
                return null;
            }

            var templateSheet = HssfWorkbook.GetSheet("Template");
            var resultSheet = HssfWorkbook.GetSheet("Statement");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            var rowId = 0;
            const int colId = 0;

            var mainTitle = " Statement - " + _builder.Username + " - From " + _builder.FromDate + " To " +
                            _builder.ToDate;

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            rowId = 2;
            var id = 0;

            if (data.Count > 0)
            {
                const int totalColumn = 4;
                var lastColumnIndex = colId + totalColumn - 1;
                double accumulatedTotal = 0;
                var ballanceResult = "OK";

                foreach (var item in data)
                {
                    var isText = false;

                    resultSheet.CopyRow(templateSheet, id % 2 == 0 ? 1 : 3, rowId, colId, lastColumnIndex);

                    if (string.Equals(item.TransType, "BAL", StringComparison.CurrentCultureIgnoreCase)
                        && item.TransDesc.ToUpper(CultureInfo.InvariantCulture) != "ACCUMULATED BALANCE")
                    {
                        isText = true;
                        if (Math.Abs(accumulatedTotal - item.TotalAmt) < 0.00001 || id == 0)
                        {
                            ballanceResult = "OK";
                        }
                        else
                        {
                            accumulatedTotal -= item.TotalAmt;
                            ballanceResult = "NOT EQUAL \nDiff: " + Utils.ToDecimal(accumulatedTotal);
                        }
                    }
                    else
                    {
                        accumulatedTotal = item.TotalAmt + accumulatedTotal;
                    }

                    List<object> values;
                    if (isText)
                    {
                        values = new List<object>
                        {
                            item.AccDate.Replace("<br/>", "\n"),
                            item.TransType + "\n- " + item.TransDesc,
                            item.TotalAmt,
                            ballanceResult
                        };
                        accumulatedTotal = item.TotalAmt;
                    }
                    else
                    {
                        values = new List<object>
                        {
                            item.AccDate.Replace("<br/>", "\n"),
                            item.TransType + "\n- " + item.TransDesc,
                            item.TotalAmt,
                            accumulatedTotal
                        };
                    }

                    resultSheet.GetRow(rowId).WriteCells(colId, values);

                    id++;
                    rowId++;
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return HssfWorkbook;
        }
    }
}
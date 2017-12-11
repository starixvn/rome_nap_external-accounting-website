namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.ExternalAccount.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class StatementPage : TemplateReport, IReportBuilder
    {
        private StatementPageBuilder _builder;

        public StatementPage(string filePath, StatementPageBuilder builder)
            : base(filePath)
        {
            this._builder = builder;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public HSSFWorkbook Build()
        {
            var data = this._builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("Statement");

            if (this._builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            string mainTitle = " Statement - " + this._builder.Username + " - From " + this._builder.FromDate + " To " + this._builder.ToDate;

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            rowId = 2;
            int id = 0;

            if (data.Count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 4;
                int lastColumnIndex = colId + totalColumn - 1;
                double accumulatedTotal = 0;
                bool isText = false;
                string ballanceResult = "OK";

                foreach (var item in data)
                {
                    isText = false;

                    if (id % 2 == 0)
                    {
                        resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                    }
                    else
                    {
                        resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                    }

                    if (item.TransType.ToUpper() == "BAL" && item.TransDesc.ToUpper() != "ACCUMULATED BALANCE") 
                    {
                        isText = true;
                        if (Math.Abs(accumulatedTotal - item.TotalAmt) < 0.00001 || id == 0)
                        {
                            ballanceResult = "OK";
                        }
                        else 
                        {
                            accumulatedTotal = accumulatedTotal - item.TotalAmt;
                            ballanceResult = "NOT EQUAL \nDiff: " + Utils.ToDecimal(accumulatedTotal);
                        }
                    }
                    else 
                    {
                        accumulatedTotal = item.TotalAmt + accumulatedTotal;
                    }

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
                            accumulatedTotal,
                        };
                    }

                    resultSheet.GetRow(rowId).WriteCells(colId, values);

                    id++;
                    rowId++;
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System;
    using System.Collections.Generic;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;
    using Fanex.ExternalAccount.Utilities;

    internal class StatementPage : TemplateReport, IReportBuilder
    {
        private StatementPageBuilder builder;

        public StatementPage(string filePath, StatementPageBuilder builder)
            : base(filePath)
        {
            this.builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var data = this.builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("Statement");

            // apply Bold on all fonts
            if (this.builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            // Header
            string mainTitle = " Statement " + this.builder.Username + " From " + this.builder.FromDate + " To " + this.builder.ToDate;

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
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

                    if (item.TransType.ToUpper() == "BAL" && item.TransDesc.ToUpper() != "ACCUMULATED BALANCE") {
                        isText = true;
                        if (Math.Abs(accumulatedTotal - item.TotalAmt) < 0.00001 || id == 0)
                        {
                            ballanceResult = "OK";
                        }
                        else {
                            accumulatedTotal = accumulatedTotal - item.TotalAmt;
                            ballanceResult = "NOT EQUAL \nDiff: " + Formater.DecFormat(accumulatedTotal, 2);
                        }
                    }
                    else {
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
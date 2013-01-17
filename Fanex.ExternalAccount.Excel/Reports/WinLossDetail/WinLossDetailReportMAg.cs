namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class WinLossDetailReportMAg : TemplateReport, IReportBuilder
    {
        private WinLossDetailReportMAgBuilder builder;

        public WinLossDetailReportMAg(string filePath, WinLossDetailReportMAgBuilder builder)
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

            ISheet templateSheet = HssfWorkbook.GetSheet("TemplateMAg");
            ISheet resultSheet = HssfWorkbook.GetSheet("WinLossDetailMAg");

            // apply Bold on all fonts
            if (this.builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            // Header
            string mainTitle = string.Empty;
            if (this.builder.FromDate.Equals(this.builder.ToDate))
            {
                mainTitle = "Agent Win Loss Detail on " + this.builder.FromDate;
            }
            else
            {
                mainTitle = "Agent Win Loss Detail From " + this.builder.FromDate + " To " + this.builder.ToDate;
            }

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 3;
            int id = 1;

            if (data.Count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 13;
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
                        item.GrossComm,
                        item.MemberWinlost,
                        item.PlayerComm,
                        item.MemberTotal,
                        item.AgWinlost, item.AgComm,
                        item.AgTotal,
                        item.MAgWinlost, 
                        item.MAgComm,
                        item.MAgTotal,
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
                    data.Sum(p => p.Company) 
                };

                resultSheet.GetRow(rowId).WriteCells(colId, values);
            }

            this.HssfWorkbook.RemoveSheetAt(this.HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }
    }
}
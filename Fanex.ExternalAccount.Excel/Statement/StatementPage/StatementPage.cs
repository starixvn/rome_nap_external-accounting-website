namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

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
            ISheet resultSheet = HssfWorkbook.GetSheet("DailyWinLoss");

            // apply Bold on all fonts
            if (this.builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            // Header
            string mainTitle = string.Empty;
            if (this.builder.RoleId == 4)
            {
                mainTitle = "Super";
            }
            else if (this.builder.RoleId == 3)
            {
                mainTitle = "Master";
            }
            else if (this.builder.RoleId == 2)
            {
                mainTitle = "Agent";
            }
            else
            {
                mainTitle = "Member";
            }

            mainTitle += " Daily Win Loss on " + this.builder.WinlostDate;

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 3;
            int id = 1;

            if (data.Count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 19;
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
                        //item.No,
                        //item.Username,
                        //item.TotalTO,
                        //item.ComTO,
                        //item.ComTOPercent,
                        //item.SAgTO,
                        //item.SAgTOPercent,
                        //item.MAgTO,
                        //item.MAgTOPercent,
                        //item.AgTO,
                        //item.AgTOPercent,
                        //item.ComWinlost,
                        //item.ComWinlostPercent,
                        //item.SAgWinlost,
                        //item.SAgWinlostPercent,
                        //item.MAgWinlost,
                        //item.MAgWinlostPercent,
                        //item.AgWinlost,
                        //item.AgWinlostPercent
                    };

                    resultSheet.GetRow(rowId).WriteCells(colId, values);

                    id++;
                    rowId++;
                }

                // total
                resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
                values = new List<object>
                {
                    string.Empty,
                    "Grand Total",
                    //data.Sum(p => p.TotalTO),
                    //data.Sum(p => p.ComTO),
                    //data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.ComTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    //data.Sum(p => p.SAgTO),
                    //data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.SAgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    //data.Sum(p => p.MAgTO),
                    //data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.MAgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    //data.Sum(p => p.AgTO),
                    //data.Sum(p => p.TotalTO) != 0 ? data.Sum(p => p.AgTO) / data.Sum(p => p.TotalTO) * 100 : 0,
                    //data.Sum(p => p.ComWinlost),
                    //data.Sum(p => p.ComTO) != 0 ? data.Sum(p => p.ComWinlost) / data.Sum(p => p.ComTO) * 100 : 0,
                    //data.Sum(p => p.SAgWinlost),
                    //data.Sum(p => p.SAgTO) != 0 ? data.Sum(p => p.SAgWinlost) / data.Sum(p => p.SAgTO) * 100 : 0,
                    //data.Sum(p => p.MAgWinlost),
                    //data.Sum(p => p.MAgTO) != 0 ? data.Sum(p => p.MAgWinlost) / data.Sum(p => p.MAgTO) * 100 : 0,
                    //data.Sum(p => p.AgWinlost),
                    //data.Sum(p => p.AgTO) != 0 ? data.Sum(p => p.AgWinlost) / data.Sum(p => p.AgTO) * 100 : 0
                };

                resultSheet.GetRow(rowId).WriteCells(colId, values);
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using Utility;

    internal class WinLossDetailReport : TemplateReport, IReportBuilder
    {
        private readonly WinLossDetailReportBuilder _builder;

        public WinLossDetailReport(string filePath, WinLossDetailReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            _builder = builder;
        }
        
        public HSSFWorkbook Build()
        {
            var data = _builder.Data;
            if (data == null)
            {
                return null;
            }

            var templateSheet = HssfWorkbook.GetSheet("Template");
            var resultSheet = HssfWorkbook.GetSheet("WinLossDetail");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            var rowId = 0;
            const int colId = 0;

            var mainTitle = BuildMainTitle();

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            rowId = 3;
            var id = 1;

            if (data.Count > 0)
            {
                const int totalColumn = 17;
                const int lastColumnIndex = colId + totalColumn - 1;

                List<object> values;
                foreach (var item in data)
                {
                    resultSheet.CopyRow(templateSheet, id % 2 == 0 ? 1 : 3, rowId, colId, lastColumnIndex);

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

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return HssfWorkbook;
        }

        private string BuildMainTitle()
        {
            var mainTitle = string.Empty;
            switch (_builder.RoleId)
            {
                case 4:
                    mainTitle = "Super";
                    break;
                case 3:
                    mainTitle = "Master";
                    break;
            }

            mainTitle += _builder.FromDate.Equals(_builder.ToDate)
                ? " Win Loss Detail on " + _builder.FromDate
                : " Win Loss Detail From " + _builder.FromDate + " To " + _builder.ToDate;

            mainTitle += " - " + Products;

            return mainTitle;
        }

    }
}
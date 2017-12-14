namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class DailyWinLossReport : TemplateReport, IReportBuilder
    {
        private readonly DailyWinLossReportBuilder _builder;

        public DailyWinLossReport(string filePath, DailyWinLossReportBuilder builder)
            : base(filePath)
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
            var resultSheet = HssfWorkbook.GetSheet("DailyWinLoss");
            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            var rowId = 0;
            const int colId = 0;
            var mainTitle = BuildMainTitle(_builder.WinlostDate, _builder.RoleId);
            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);
            if (data.Count > 0)
            {
                WriteCells(resultSheet, templateSheet, colId);
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return HssfWorkbook;
        }

        private void WriteCells(ISheet resultSheet, ISheet templateSheet, int colId)
        {
            var id = 1;
            var rowId = 3;
            const int totalColumn = 19;
            var lastColumnIndex = colId + totalColumn - 1;
            foreach (var item in _builder.Data)
            {
                resultSheet.CopyRow(templateSheet, id % 2 == 0 ? 1 : 3, rowId, colId, lastColumnIndex);

                var values = new List<object>
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

            WriteTotal(resultSheet, templateSheet, rowId, colId, lastColumnIndex);
        }

        private void WriteTotal(ISheet resultSheet, ISheet templateSheet, int rowId, int colId,
            int lastColumnIndex)
        {
            var data = _builder.Data;
            resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
            var values = new List<object>
            {
                string.Empty,
                "Grand Total",
                data.Sum(p => p.TotalTO),
                data.Sum(p => p.ComTO),
                !data.Sum(p => p.TotalTO).Equals(0)
                    ? data.Sum(p => p.ComTO) / data.Sum(p => p.TotalTO) * 100
                    : 0,
                data.Sum(p => p.SAgTO),
                !data.Sum(p => p.TotalTO).Equals(0)
                    ? data.Sum(p => p.SAgTO) / data.Sum(p => p.TotalTO) * 100
                    : 0,
                data.Sum(p => p.MAgTO),
                !data.Sum(p => p.TotalTO).Equals(0)
                    ? data.Sum(p => p.MAgTO) / data.Sum(p => p.TotalTO) * 100
                    : 0,
                data.Sum(p => p.AgTO),
                !data.Sum(p => p.TotalTO).Equals(0)
                    ? data.Sum(p => p.AgTO) / data.Sum(p => p.TotalTO) * 100
                    : 0,
                data.Sum(p => p.ComWinlost),
                !data.Sum(p => p.ComTO).Equals(0)
                    ? data.Sum(p => p.ComWinlost) / data.Sum(p => p.ComTO) * 100
                    : 0,
                data.Sum(p => p.SAgWinlost),
                !data.Sum(p => p.SAgTO).Equals(0)
                    ? data.Sum(p => p.SAgWinlost) / data.Sum(p => p.SAgTO) * 100
                    : 0,
                data.Sum(p => p.MAgWinlost),
                !data.Sum(p => p.MAgTO).Equals(0)
                    ? data.Sum(p => p.MAgWinlost) / data.Sum(p => p.MAgTO) * 100
                    : 0,
                data.Sum(p => p.AgWinlost),
                !data.Sum(p => p.AgTO).Equals(0)
                    ? data.Sum(p => p.AgWinlost) / data.Sum(p => p.AgTO) * 100
                    : 0            };
            resultSheet.GetRow(rowId).WriteCells(colId, values);
        }

        private static string BuildMainTitle(string winlostDate, int roleId)
            => $"{(roleId == 5 ? "Super" : roleId == 4 ? "Master" : roleId == 3 ? "Agent" : "Member")} Daily Win Loss on {winlostDate}";

    }
}
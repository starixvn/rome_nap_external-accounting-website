namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Shared;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class DailyWinLossReport : TemplateReport, IReportBuilder
    {
        private readonly DailyWinLossReportBuilder _builder;
        private int LastColumnIndex { get; }
        private bool IsExcludeLevelType { get; }

        public DailyWinLossReport(string filePath, DailyWinLossReportBuilder builder)
            : base(filePath)
        {
            _builder = builder;
            IsExcludeLevelType = _builder.RoleId != Role.Super && _builder.RoleId != Role.Master;
            LastColumnIndex = IsExcludeLevelType ? 18 : 19;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("DailyWinLoss");
            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);
            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var index = 1;
            var toRowId = 3;

            foreach (var item in _builder.Data)
            {
                var fromRowId = index % 2 == 0 ? OddRowIndex : EvenRowIndex;
                resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, LastColumnIndex);

                var values = new List<object>
                {
                    item.No,
                    item.Username,
                    GetLevelType(item.RoleId),
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
                if (IsExcludeLevelType)
                {
                    values.RemoveAt(2);
                }
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values);
                index++;
                toRowId++;
            }

            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var data = _builder.Data;
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, LastColumnIndex);
            var values = new List<object>
            {
                string.Empty,
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
            if (IsExcludeLevelType)
            {
                values.RemoveAt(0);
            }
            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var downlineLevel = _builder.RoleId.DirectDownlineLevel();

            var mainTitle = $"{downlineLevel.DisplayRoleName()} Daily Win Loss on {_builder.WinlostDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }

        private static string GetLevelType(int roleId)
        {
            switch (roleId)
            {
                case Role.Master:
                    return "Master";
                case Role.Agent:
                    return "Agent";
                default:
                    return "Direct member";
            }
        }
    }
}
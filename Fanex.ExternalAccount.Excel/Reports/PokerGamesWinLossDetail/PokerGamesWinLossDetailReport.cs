namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System.Linq;
    using Core.Shared;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class PokerGamesWinLossDetailReport : TemplateReport, IReportBuilder
    {
        private readonly PokerGamesWinLossDetailReportBuilder _builder;
        private const int LastColumnIndex = 17;

        public PokerGamesWinLossDetailReport(string filePath, PokerGamesWinLossDetailReportBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build() => Build("PokerGamesWinLoss", _builder.IsBold);

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var toRowId = WriteBodyReport(resultSheet, _builder.Data.Select(item => item.GetExcelColumns()), LastColumnIndex);
            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var total = new Fanex.ExternalAccount.DTO.Report.PokerGamesWinLossDetail { Username = "Grand Total" };
            foreach (var p in _builder.Data)
            {
                total.Sum(p);
            }

            WriteTotal(resultSheet, toRowIndex, total.GetExcelColumns(), LastColumnIndex);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.RoleId.IsSuper() ? "Super " : _builder.RoleId.IsMaster() ? "Master " : string.Empty;

            mainTitle += _builder.FromDate.Equals(_builder.ToDate)
                ? $"Poker Games Win Loss on {_builder.FromDate}"
                : $"Poker Games Win Loss From {_builder.FromDate} To {_builder.ToDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
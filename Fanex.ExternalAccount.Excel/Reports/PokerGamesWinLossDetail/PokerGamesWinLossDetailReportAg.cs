namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class PokerGamesWinLossDetailReportAg : TemplateReport, IReportBuilder
    {
        private readonly MemberPokerGamesWinLossDetailReportBuilder _builder;
        private const int LastColumnIndex = 11;

        public PokerGamesWinLossDetailReportAg(string filePath, MemberPokerGamesWinLossDetailReportBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build() => Build("PokerGamesWinLossAg", _builder.IsBold);

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var toRowId = WriteBodyReport(resultSheet, _builder.Data.Select(item => item.GetExcelColumns()), LastColumnIndex);
            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var total = new Fanex.ExternalAccount.DTO.Report.PokerGamesWinLossDetailAg { Username = "Grand Total" };
            foreach (var p in _builder.Data)
            {
                total.Sum(p);
            }

            WriteTotal(resultSheet, toRowIndex, total.GetExcelColumns(), LastColumnIndex);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Member Poker Games Win Loss on {_builder.FromDate}"
                : $"Member Poker Games Win Loss From {_builder.FromDate} To {_builder.ToDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
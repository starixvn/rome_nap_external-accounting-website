namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class PokerGamesWinLossDetailReportMAg : TemplateReport, IReportBuilder
    {
        private readonly AgentPokerGamesWinLossDetailReportBuilder _builder;
        private const int LastColumnIndex = 14;

        public PokerGamesWinLossDetailReportMAg(string filePath, AgentPokerGamesWinLossDetailReportBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build() => Build("PokerGamesWinLossMAg", _builder.IsBold);

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var toRowId = WriteBodyReport(resultSheet, _builder.Data.Select(item => item.GetExcelColumns()), LastColumnIndex);
            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var total = new Fanex.ExternalAccount.DTO.Report.PokerGamesWinLossDetailMAg { Username = "Grand Total" };
            foreach (var p in _builder.Data)
            {
                total.Sum(p);
            }

            WriteTotal(resultSheet, toRowIndex, total.GetExcelColumns(), LastColumnIndex);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Agent Poker Games Win Loss on {_builder.FromDate}"
                : $"Agent Poker Games Win Loss From {_builder.FromDate} To {_builder.ToDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System.Collections.Generic;
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

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("PokerGamesWinLossMAg");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);
            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        private void WriteBodyReport(ISheet resultSheet)
        {
            var index = 1;
            var toRowId = 3;

            foreach (var item in _builder.Data)
            {
                var fromRowId = index % 2 == 0 ? OddRowIndex : EvenRowIndex;
                resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, LastColumnIndex);

                var values = new List<object>
                {
                    item.Username,
                    item.Turnover,
                    item.NetTurnover,
                    item.TaxComm,
                    item.MemberWinLoss,
                    item.MemberInsuranceGemsWinLoss,
                    item.MemberComm,
                    item.MemberTotal,
                    item.AgInsuranceGemsWinLoss,
                    item.AgComm,
                    item.AgTotal,
                    item.MAgInsuranceGemsWinLoss,
                    item.MAgComm,
                    item.MAgTotal,
                    item.Company
                };

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
                "Grand Total",
                data.Sum(p => p.Turnover),
                data.Sum(p => p.NetTurnover),
                data.Sum(p => p.TaxComm),
                data.Sum(p => p.MemberWinLoss),
                data.Sum(p => p.MemberInsuranceGemsWinLoss),
                data.Sum(p => p.MemberComm),
                data.Sum(p => p.MemberTotal),
                data.Sum(p => p.AgInsuranceGemsWinLoss),
                data.Sum(p => p.AgComm),
                data.Sum(p => p.AgTotal),
                data.Sum(p => p.MAgInsuranceGemsWinLoss),
                data.Sum(p => p.MAgComm),
                data.Sum(p => p.MAgTotal),
                data.Sum(p => p.Company)
            };

            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        private void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Agent Poker Games Win Loss on {_builder.FromDate}"
                : $"Agent Poker Games Win Loss From {_builder.FromDate} To {_builder.ToDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
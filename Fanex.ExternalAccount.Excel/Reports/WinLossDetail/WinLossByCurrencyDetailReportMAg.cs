namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class WinLossByCurrencyDetailReportMAg : TemplateReport, IReportBuilder
    {
        private readonly AgentWinLossByCurrencyDetailReportBuilder _builder;
        private const int LastColumnIndex = 17;

        public WinLossByCurrencyDetailReportMAg(string filePath, AgentWinLossByCurrencyDetailReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("WinLossDetailMAg");

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
                    item.Username,
                    TemplateSettings.GetLevelType(item.RoleId),
                    item.Currency,
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
                    item.Company,
                    item.RewardAmount,
                    item.RewardCurrency ?? string.Empty
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values);
                index++;
                toRowId++;
            }

            WriteTotal(resultSheet, toRowId);
        }
        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            foreach (var item in _builder.DataTotal)
            {
                resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, LastColumnIndex);
                var values = new List<object>
                {
                    item.Username,
                    null,
                    null,
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
                    item.Company,
                    item.RewardAmount,
                    item.RewardCurrency ?? string.Empty
                };

                resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
                toRowIndex++;
            }
        }
        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Agent Win Loss Detail on {_builder.FromDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}"
                : $"Agent Win Loss Detail From {_builder.FromDate} To {_builder.ToDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
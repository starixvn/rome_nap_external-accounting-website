namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using Fanex.ExternalAccount.Core.Shared;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using Utility;

    internal class WinLossByCurrencyDetailReport : TemplateReport, IReportBuilder
    {
        private readonly WinLossByCurrencyDetailReportBuilder _builder;
        private const int LastColumnIndex = 19;
        public WinLossByCurrencyDetailReport(string filePath, WinLossByCurrencyDetailReportBuilder builder, string products = null) : base(filePath, products)
        {
            _builder = builder;
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
                    item.SAgWinlost,
                    item.SAgComm,
                    item.SAgTotal,
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
                    item.Company,
                    item.RewardAmount,
                    item.RewardCurrency ?? string.Empty
                };

                resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
                toRowIndex++;
            }
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("WinLossDetail");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }
        public override void BuildMainTitle(ISheet resultSheet)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            var mainTitle = _builder.RoleId.IsSuper() ? "Super" : _builder.RoleId.IsMaster() ? "Master" : string.Empty;
#pragma warning restore S3358 // Ternary operators should not be nested

            mainTitle += _builder.FromDate.Equals(_builder.ToDate)
                ? $" Win Loss Detail on {_builder.FromDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}"
                : $" Win Loss Detail From {_builder.FromDate} To {_builder.ToDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}

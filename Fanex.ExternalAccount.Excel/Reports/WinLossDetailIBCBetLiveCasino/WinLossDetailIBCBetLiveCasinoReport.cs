namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    internal class WinLossDetailIBCBetLiveCasinoReport : TemplateReport, IReportBuilder
    {
        private readonly WinLossDetailIBCBetLiveCasinoReportBuilder _builder;
        private const int LastColumnIndex = 13;

        public WinLossDetailIBCBetLiveCasinoReport(string filePath, WinLossDetailIBCBetLiveCasinoReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("WinLossDetailIBCBetLiveCasino");

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
                    item.Turnover,
                    item.NetTurnover,
                    item.GrossComm,
                    item.MemberWinlost,
                    item.PlayerComm,
                    item.MemberTotal,
                    item.SAgWinlost,
                    item.SAgComm,
                    item.SAgTotal,
                    item.ExtWinlost,
                    item.ExtComm,
                    item.ExtTotal,
                    item.NetCompany,
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
                data.Sum(p => p.GrossComm),
                data.Sum(p => p.MemberWinlost),
                data.Sum(p => p.PlayerComm),
                data.Sum(p => p.MemberTotal),
                data.Sum(p => p.SAgWinlost),
                data.Sum(p => p.SAgComm),
                data.Sum(p => p.SAgTotal),
                data.Sum(p => p.ExtWinlost),
                data.Sum(p => p.ExtComm),
                data.Sum(p => p.ExtTotal),
                data.Sum(p => p.NetCompany),
            };

            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            string mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Win Loss Detail for IBCBet Live Casino on {_builder.FromDate}"
                : $"Win Loss Detail for IBCBet Live Casino From {_builder.FromDate} To {_builder.ToDate}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
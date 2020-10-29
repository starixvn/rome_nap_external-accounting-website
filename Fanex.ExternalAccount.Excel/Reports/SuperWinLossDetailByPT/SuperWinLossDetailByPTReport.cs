namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    internal class SuperWinLossDetailByPTReport : TemplateReport, IReportBuilder
    {
        private readonly SuperWinLossDetailByPTReportBuilder _builder;
        private const int LastColumnIndex = 16;

        public SuperWinLossDetailByPTReport(string filePath, SuperWinLossDetailByPTReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("SuperWinLossDetailByPT");

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
                resultSheet.CopyRow(TemplateSheet, 1, toRowId, FirstColumnIndex, LastColumnIndex);

                var values = new List<object>
                {
                    item.Username,
                    item.Total.Turnover,
                    item.Total.NetTurnover,
                    item.Total.GrossComm,
                    item.Total.MemberWinlost,
                    item.Total.PlayerComm,
                    item.Total.MemberTotal,
                    item.Total.AgWinlost,
                    item.Total.AgComm,
                    item.Total.AgTotal,
                    item.Total.MAgWinlost,
                    item.Total.MAgComm,
                    item.Total.MAgTotal,
                    item.Total.SAgWinlost,
                    item.Total.SAgComm,
                    item.Total.SAgTotal,
                    item.Total.Company
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values);
                index++;
                toRowId++;

                foreach (var childRow in item.WinLossByProducts)
                {
                    resultSheet.CopyRow(TemplateSheet, 3, toRowId, FirstColumnIndex, LastColumnIndex);

                    var values01 = new List<object>
                    {
                        childRow.PositionTaking,
                        childRow.Turnover,
                        childRow.NetTurnover,
                        childRow.GrossComm,
                        childRow.MemberWinlost,
                        childRow.PlayerComm,
                        childRow.MemberTotal,
                        childRow.AgWinlost,
                        childRow.AgComm,
                        childRow.AgTotal,
                        childRow.MAgWinlost,
                        childRow.MAgComm,
                        childRow.MAgTotal,
                        childRow.SAgWinlost,
                        childRow.SAgComm,
                        childRow.SAgTotal,
                        childRow.Company
                    };

                    resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values01);
                    index++;
                    toRowId++;
                }
            }

            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var data = _builder.Grandtotal;
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, LastColumnIndex);
            var values = new List<object>
            {
                "Grand Total",
                data.Turnover,
                data.NetTurnover,
                data.GrossComm,
                data.MemberWinlost,
                data.PlayerComm,
                data.MemberTotal,
                data.AgWinlost,
                data.AgComm,
                data.AgTotal,
                data.MAgWinlost,
                data.MAgComm,
                data.MAgTotal,
                data.SAgWinlost,
                data.SAgComm,
                data.SAgTotal,
                data.Company
            };

            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
           var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Super Win Loss Detail By PT on {_builder.FromDate}"
                : $"Super Win Loss Detail By PT {_builder.FromDate} To {_builder.ToDate}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
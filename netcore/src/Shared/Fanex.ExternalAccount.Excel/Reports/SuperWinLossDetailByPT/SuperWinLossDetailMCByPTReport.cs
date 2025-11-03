namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DTO.Report;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using Utility;

    internal class SuperWinLossDetailMCByPTReport : TemplateReport, IReportBuilder
    {
        private readonly SuperWinLossDetailByMCPTReportBuilder _builder;
        private const int LastColumnIndex = 17;
        private const int parentRowTemplateIndex = 1;
        private const int childRowTemplateIndex = 3;

        public SuperWinLossDetailMCByPTReport(
            string filePath,
            SuperWinLossDetailByMCPTReportBuilder builder,
            string products = null)
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
            var rowIndex = 1;
            var toRowId = 3;

            foreach (var winlossByUsername in _builder.WinLossDetailByUsernames)
            {
                resultSheet.CopyRow(TemplateSheet, parentRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
                var winlossUsernameTotal = winlossByUsername.Total;

                var parentRowValues = new List<object>
                {
                    winlossUsernameTotal.Username,
                    winlossUsernameTotal.CurrencyName,
                    winlossUsernameTotal.Turnover,
                    winlossUsernameTotal.NetTurnover,
                    winlossUsernameTotal.GrossComm,
                    winlossUsernameTotal.MemberWinlost,
                    winlossUsernameTotal.MemberComm,
                    winlossUsernameTotal.MemberTotal,
                    winlossUsernameTotal.AgWinlost,
                    winlossUsernameTotal.AgComm,
                    winlossUsernameTotal.AgTotal,
                    winlossUsernameTotal.MAgWinlost,
                    winlossUsernameTotal.MAgComm,
                    winlossUsernameTotal.MAgTotal,
                    winlossUsernameTotal.SAgWinlost,
                    winlossUsernameTotal.SAgComm,
                    winlossUsernameTotal.SAgTotal,
                    winlossUsernameTotal.Company
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, parentRowValues);
                rowIndex++;
                toRowId++;

                foreach (var winlossByProduct in winlossByUsername.WinLossByProducts)
                {
                    resultSheet.CopyRow(TemplateSheet, childRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);

                    var childRowValues = new List<object>
                    {
                        winlossByProduct.PositionTakingText,
                        null,
                        winlossByProduct.Turnover,
                        winlossByProduct.NetTurnover,
                        winlossByProduct.GrossComm,
                        winlossByProduct.MemberWinlost,
                        winlossByProduct.MemberComm,
                        winlossByProduct.MemberTotal,
                        winlossByProduct.AgWinlost,
                        winlossByProduct.AgComm,
                        winlossByProduct.AgTotal,
                        winlossByProduct.MAgWinlost,
                        winlossByProduct.MAgComm,
                        winlossByProduct.MAgTotal,
                        winlossByProduct.SAgWinlost,
                        winlossByProduct.SAgComm,
                        winlossByProduct.SAgTotal,
                        winlossByProduct.Company
                    };

                    resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, childRowValues);
                    rowIndex++;
                    toRowId++;
                }
            }

            WriteGrandTotal(resultSheet, _builder.TotalByCurrencies, toRowId);
        }

        private void WriteGrandTotal(ISheet resultSheet, List<SuperWinLossDetailMCByProductDTO> totalByCurrencies, int grandTotalRowIndex)
        {
            foreach (var totalByCurrency in totalByCurrencies)
            {
                resultSheet.CopyRow(
                TemplateSheet,
                FirstColumnTotalIndex,
                grandTotalRowIndex,
                FirstColumnIndex,
                LastColumnIndex);

                var grandtotalRowValues = new List<object>
            {
                totalByCurrency.TotalName,
                null,
                totalByCurrency.Turnover,
                totalByCurrency.NetTurnover,
                totalByCurrency.GrossComm,
                totalByCurrency.MemberWinlost,
                totalByCurrency.MemberComm,
                totalByCurrency.MemberTotal,
                totalByCurrency.AgWinlost,
                totalByCurrency.AgComm,
                totalByCurrency.AgTotal,
                totalByCurrency.MAgWinlost,
                totalByCurrency.MAgComm,
                totalByCurrency.MAgTotal,
                totalByCurrency.SAgWinlost,
                totalByCurrency.SAgComm,
                totalByCurrency.SAgTotal,
                totalByCurrency.Company
            };

                resultSheet.GetRow(grandTotalRowIndex).WriteCells(FirstColumnIndex, grandtotalRowValues);
                grandTotalRowIndex++;
            }
            
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                 ? $"Super Win Loss Detail By PT on {_builder.FromDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}"
                 : $"Super Win Loss Detail By PT {_builder.FromDate} To {_builder.ToDate} in {(string.IsNullOrWhiteSpace(_builder.BaseCurrency) ? "customer currency" : _builder.BaseCurrency)}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
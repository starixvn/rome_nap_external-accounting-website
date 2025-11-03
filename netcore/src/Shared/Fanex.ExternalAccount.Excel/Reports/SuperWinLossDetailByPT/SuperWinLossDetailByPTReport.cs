namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DTO.Report;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using Utility;

    internal class SuperWinLossDetailByPTReport : TemplateReport, IReportBuilder
    {
        private readonly SuperWinLossDetailByPTReportBuilder _builder;
        private const int LastColumnIndex = 16;
        private const int parentRowTemplateIndex = 1;
        private const int childRowTemplateIndex = 3;

        public SuperWinLossDetailByPTReport(
            string filePath, 
            SuperWinLossDetailByPTReportBuilder builder, 
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
                    winlossUsernameTotal.Username ?? "Dormant Accounts",
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

            WriteGrandTotal(resultSheet, _builder.Grandtotal, toRowId);
        }

        private void WriteGrandTotal(ISheet resultSheet, SuperWinLossDetailByProductDTO grantotal, int grandTotalRowIndex)
        {
            resultSheet.CopyRow(
                TemplateSheet, 
                FirstColumnTotalIndex, 
                grandTotalRowIndex, 
                FirstColumnIndex, 
                LastColumnIndex);

            var grandtotalRowValues = new List<object>
            {
                "Grand Total",
                grantotal.Turnover,
                grantotal.NetTurnover,
                grantotal.GrossComm,
                grantotal.MemberWinlost,
                grantotal.MemberComm,
                grantotal.MemberTotal,
                grantotal.AgWinlost,
                grantotal.AgComm,
                grantotal.AgTotal,
                grantotal.MAgWinlost,
                grantotal.MAgComm,
                grantotal.MAgTotal,
                grantotal.SAgWinlost,
                grantotal.SAgComm,
                grantotal.SAgTotal,
                grantotal.Company
            };

            resultSheet.GetRow(grandTotalRowIndex).WriteCells(FirstColumnIndex, grandtotalRowValues);
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
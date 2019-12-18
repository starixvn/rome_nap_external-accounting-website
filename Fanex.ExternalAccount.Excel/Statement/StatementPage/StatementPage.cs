namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utilities;
    using Utility;

    internal class StatementPage : TemplateReport, IReportBuilder
    {
        private readonly StatementPageBuilder _builder;
        private const int LastColumnIndex = 3;

        public StatementPage(string filePath, StatementPageBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("Statement");

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
            var index = 0;
            var toRowId = 2;

            double accumulatedTotal = 0;
            var ballanceResult = "OK";

            foreach (var item in _builder.Data)
            {
                var isText = false;
#pragma warning disable S109 // Magic numbers should not be used

                resultSheet.CopyRow(TemplateSheet, index % 2 == 0 ? OddRowIndex : EvenRowIndex, toRowId, FirstColumnIndex, LastColumnIndex);

                if (string.Equals(item.TransType, "BAL", StringComparison.CurrentCultureIgnoreCase)
                    && item.TransDesc.ToUpper(CultureInfo.InvariantCulture) != "ACCUMULATED BALANCE")
                {
                    isText = true;
                    if (Math.Abs(accumulatedTotal - item.TotalAmt) < 0.00001 || index == 0)
#pragma warning restore S109 // Magic numbers should not be used
                    {
                        ballanceResult = "OK";
                    }
                    else
                    {
                        accumulatedTotal -= item.TotalAmt;
                        ballanceResult = "NOT EQUAL \nDiff: " + Utils.ToDecimal(accumulatedTotal).Format();
                    }
                }
                else
                {
                    accumulatedTotal = item.TotalAmt + accumulatedTotal;
                }

                List<object> values;
                if (isText)
                {

                    values = new List<object>
                        {
                            item.AccDate.Replace("<br/>", "\n"),
                            item.TransType + "\n- " + item.TransDesc,
                            item.TotalAmt,
                            ballanceResult
                        };
                    accumulatedTotal = item.TotalAmt;
                }
                else
                {
                    values = new List<object>
                        {
                            item.AccDate.Replace("<br/>", "\n"),
                            item.TransType + "\n- " + item.TransDesc,
                            item.TotalAmt,
                            accumulatedTotal
                        };
                }

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values);

                index++;
                toRowId++;
            }

        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = $" Statement - {_builder.Username} - From {_builder.FromDate} To {_builder.ToDate}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
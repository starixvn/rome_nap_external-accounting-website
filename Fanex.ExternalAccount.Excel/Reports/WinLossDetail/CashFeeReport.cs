using Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport;
using Fanex.ExternalAccount.Excel.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetail
{
    public class CashFeeReport : TemplateReport, IReportBuilder
    {
        private readonly CashFeeBuilder _builder;

        private readonly string _userName = string.Empty;

        private const int LastColumnIndex = 4;

        public CashFeeReport(string filePath, CashFeeBuilder builder, string products = null, string userName = null)
            : base(filePath, products)
        {
            _builder = builder;
            _userName = userName;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("CashServiceBilling");

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
            var fromRowId = 0;
            var toRowId = 2;

            foreach (var groupItem in _builder.Data.FeeGroups)
            {
                fromRowId = 1;
                resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, LastColumnIndex);

                var groupValues = new List<object>
                {
                    groupItem.Total.DisplayType,
                    string.Empty,
                    groupItem.Total.Count,
                    groupItem.Total.Agent,
                    groupItem.Total.Company
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, groupValues);
                toRowId++;

                foreach(var feeItem in groupItem.FeeItems)
                {
                    fromRowId = 3;
                    resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, LastColumnIndex);

                    var feeValues = new List<object>
                    {
                        feeItem.DisplayType,
                        feeItem.Fee,
                        feeItem.Count,
                        feeItem.Agent,
                        feeItem.Company
                    };

                    resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, feeValues);
                    toRowId++;
                }
            }

            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var data = _builder.Data.GrandTotal;
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, LastColumnIndex);
            var values = new List<object>
            {
                data.DisplayType,
                string.Empty,
                string.Empty,
                data.Agent,
                data.Company
            };

            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"CASH SERVICE BILLING on {_builder.FromDate} - {_userName}"
                : $"CASH SERVICE BILLING From {_builder.FromDate} To {_builder.ToDate} - {_userName}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}

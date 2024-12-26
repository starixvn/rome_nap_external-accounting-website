namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Fanex.CustomerSetting.Module.Core.Extensions;
    using Fanex.ExternalAccount.Excel;
    using Fanex.ExternalAccount.Excel.Utility;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Models;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class WhiteLabelSettlementExcel : TemplateReport, IReportBuilder
    {
        private const int LastColumnIndex = 19;

        public WhiteLabelSettlementExcel()
            : base(HttpContext.Current.Server.MapPath("~/ExcelTemplates/Reports/WhiteLabelSettlement.xls"))
        {
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public SettlementResult ReportData { get; set; }

        public MemoryStream BuildMemoryStream()
        {
            Build();
            var memoryStream = new MemoryStream();
            HssfWorkbook.Write(memoryStream);

            return memoryStream;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("Settlement");

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var month = FromDate == ToDate
                ? $"on {ToMonthYear(FromDate)}"
                : $"from {ToMonthYear(FromDate)} to {ToMonthYear(ToDate)}";
            var title = $"White Label Settlement Report {month}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(title);
        }
        private string ToMonthYear(DateTime date) => date.ToString("MMM, yyyy");
        public override void WriteBodyReport(ISheet resultSheet)
        {
            var currentRowIndex = 3;

            foreach (var item in ReportData.Items)
            {
                resultSheet.CopyRow(TemplateSheet, EvenRowIndex, currentRowIndex, FirstColumnIndex, LastColumnIndex);
                var cells = new List<object>
                {
                    item.Month,
                    item.WhiteLabel,
                    item.Agent,
                    item.Master,
                    item.Super,
                    item.MemberWL,
                    item.MemberPromotion,
                    item.LossCarryforward,
                    item.WhiteLabelWinLoss,
                    item.WhiteLabelNetWinLoss,
                    item.WhiteLabelFee,
                    item.WhiteLabelPromotionSettlement,
                    item.WhiteLabelPromotionShare,
                    item.WhiteLabelTotal,
                    item.MasterEarning,
                    item.SuperEarning,
                    item.HeadlineEarning,
                    item.CompanyIncome,
                    item.CompanyPromotionShare,
                    item.CompanyTotal
                };

                resultSheet.GetRow(currentRowIndex).WriteCells(FirstColumnIndex, cells);
                currentRowIndex++;
            }

            WriteTotal(resultSheet, currentRowIndex);
        }

        private void WriteTotal(ISheet resultSheet, int totalRowIndex)
        {
            var total = ReportData.RowTotal;
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, totalRowIndex, FirstColumnIndex, LastColumnIndex);
            var cells = new List<object>
            {
                "Grand Total",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                total.MemberWL,
                total.MemberPromotion,
                total.LossCarryforward,
                total.WhiteLabelWinLoss,
                total.WhiteLabelNetWinLoss,
                total.WhiteLabelFee,
                total.WhiteLabelPromotionSettlement,
                total.WhiteLabelPromotionShare,
                total.WhiteLabelTotal,
                total.MasterEarning,
                total.SuperEarning,
                total.HeadlineEarning,
                total.CompanyIncome,
                total.CompanyPromotionShare,
                total.CompanyTotal
            };

            resultSheet.GetRow(totalRowIndex).WriteCells(FirstColumnIndex, cells);
        }
    }
    
}

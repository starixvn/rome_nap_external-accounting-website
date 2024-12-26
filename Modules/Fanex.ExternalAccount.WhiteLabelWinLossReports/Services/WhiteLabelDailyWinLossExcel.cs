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

    public class WhiteLabelDailyWinLossExcel : TemplateReport, IReportBuilder
    {
        private const int LastColumnIndex = 6;

        public WhiteLabelDailyWinLossExcel()
            : base(HttpContext.Current.Server.MapPath("~/ExcelTemplates/Reports/WhiteLabelDailyWinLoss.xls"))
        {
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DailyWinLossResult ReportData { get; set; }

        public MemoryStream BuildMemoryStream()
        {
            Build();
            var memoryStream = new MemoryStream();
            HssfWorkbook.Write(memoryStream);

            return memoryStream;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("DailyWinLoss");

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var date = FromDate == ToDate
                ? $"on {FromDate.ToShortFormatedDate()}"
                : $"from {FromDate.ToShortFormatedDate()} to {ToDate.ToShortFormatedDate()}";
            var title = $"White Label Daily Win Loss Report {date}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(title);
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var currentRowIndex = 2;

            foreach (var item in ReportData.Items)
            {
                resultSheet.CopyRow(TemplateSheet, EvenRowIndex, currentRowIndex, FirstColumnIndex, LastColumnIndex);
                var cells = new List<object>
                {
                    item.WinLostDate,
                    item.WhiteLabelName,
                    item.Agent,
                    item.Master,
                    item.Super,
                    item.MemberWinLoss,
                    item.Promotion
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
                total.MemberWinLoss,
                total.Promotion
            };

            resultSheet.GetRow(totalRowIndex).WriteCells(FirstColumnIndex, cells);
        }
    }
}

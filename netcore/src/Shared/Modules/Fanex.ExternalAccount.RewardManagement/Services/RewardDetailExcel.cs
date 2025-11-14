namespace Fanex.ExternalAccount.RewardManagement.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Fanex.ExternalAccount.Core.Shared.Extensions;
    using Fanex.ExternalAccount.Excel;
    using Fanex.ExternalAccount.Excel.Utility;
    using Fanex.ExternalAccount.RewardManagement.Models;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class RewardDetailExcel : TemplateReport, IReportBuilder
    {
        private const int LastColumnIndex = 16;
        private const int PaymentNotReceivedRowIndex = 1;
        private const int PaymentReceivedRowIndex = 3;

        public RewardDetailExcel() : base(Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplates", "Reports", "RewardDetail.xls"))
        {
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public RewardReport ReportData { get; set; }

        public MemoryStream BuildMemoryStream()
        {
            Build();
            var memoryStream = new MemoryStream();
            HssfWorkbook.Write(memoryStream);

            return memoryStream;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("RewardDetail");

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
            var title = $"Reward Detail {date}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(title);
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var currentRowIndex = 3;

            foreach (var item in ReportData.Items)
            {
                resultSheet.CopyRow(TemplateSheet,
                    item.MemberFeedBackMapped == "Not Received" ? PaymentNotReceivedRowIndex : PaymentReceivedRowIndex,
                    currentRowIndex, FirstColumnIndex, LastColumnIndex);
                var cells = new List<object>
                {
                    item.CreatedDate,
                    item.ProgramName,
                    item.MemberUsername,
                    item.MemberAmount,
                    item.MemberCurrency,
                    item.MemberPaymentStatusMapped,
                    item.MemberPaymentDateString,
                    item.MemberFeedBackMapped,
                    item.AgentUsername,
                    item.AgentPaymentStatusMapped,
                    item.AgentPaymentDateString,
                    item.MasterUsername,
                    item.MasterPaymentStatusMapped,
                    item.MasterPaymentDateString,
                    item.SuperUsername,
                    item.SuperPaymentStatusMapped,
                    item.SuperPaymentDateString
                };

                resultSheet.GetRow(currentRowIndex).WriteCells(FirstColumnIndex, cells);
                currentRowIndex++;
            }
        }
    }
}
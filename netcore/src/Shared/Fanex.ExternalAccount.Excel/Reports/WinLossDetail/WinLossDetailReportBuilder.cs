namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class WinLossDetailReportBuilder : IWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public WinLossDetailReportBuilder(
            List<WinLossDetail> winLossDetail,
            int uplineRoleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetail;
            UplineRoleId = uplineRoleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<WinLossDetail> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }
        public int UplineRoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailPath(UplineRoleId);
            var mem = new MemoryStream();

            using (var report = new WinLossDetailReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
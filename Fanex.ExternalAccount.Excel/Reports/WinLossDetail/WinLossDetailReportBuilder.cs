namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class WinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public WinLossDetailReportBuilder(
            List<WinLossDetail> winLossDetail,
            bool isBold,
            int roleId,
            DateTime fromDate,
            DateTime toDate)
        {
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
            RoleId = roleId;
            Data = winLossDetail;
        }

        public List<WinLossDetail> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailPath(RoleId);
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
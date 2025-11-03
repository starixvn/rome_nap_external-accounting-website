namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class MemberWinLossDetailReportBuilder : IWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public MemberWinLossDetailReportBuilder(
            List<WinLossDetailAg> winLossDetailAg,
            int uplineRoleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetailAg;
            UplineRoleId = uplineRoleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public MemberWinLossDetailReportBuilder(
            int roleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            RoleId = roleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<WinLossDetailAg> Data { get; }

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
            using (var report = new WinLossDetailReportAg(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class MemberWinLossByCurrencyDetailReportBuilder : IWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public MemberWinLossByCurrencyDetailReportBuilder(
            List<WinLossByCurrencyDetailAg> winLossDetailAg,
            List<WinLossByCurrencyDetailAg> winLossTotal,
            int uplineRoleId,
            DateTime fromDate,
            DateTime toDate,
            string baseCurrency,
            bool isBold = false)
        {
            Data = winLossDetailAg;
            DataTotal = winLossTotal;
            UplineRoleId = uplineRoleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            BaseCurrency = baseCurrency;
            IsBold = isBold;
        }

        public MemberWinLossByCurrencyDetailReportBuilder(
            int roleId,
            DateTime fromDate,
            DateTime toDate,
            string baseCurrency,
            bool isBold = false)
        {
            RoleId = roleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            BaseCurrency = baseCurrency;
            IsBold = isBold;
        }
        public List<WinLossByCurrencyDetailAg> Data { get; }

        public List<WinLossByCurrencyDetailAg> DataTotal { get; }

        public bool IsBold { get; }

        public int RoleId { get; }
        public int UplineRoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public string BaseCurrency { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossByCurrencyDetailPath(UplineRoleId);

            var mem = new MemoryStream();
            using (var report = new WinLossByCurrencyDetailReportAg(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}

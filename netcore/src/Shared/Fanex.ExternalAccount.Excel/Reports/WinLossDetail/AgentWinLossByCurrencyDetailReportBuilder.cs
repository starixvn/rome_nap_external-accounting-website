namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class AgentWinLossByCurrencyDetailReportBuilder : IWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public AgentWinLossByCurrencyDetailReportBuilder(
            List<WinLossByCurrencyDetailMAg> winLossDetailMAg,
            List<WinLossByCurrencyDetailMAg> winLossTotal,
            int uplineRoleId,
            DateTime fromDate,
            DateTime toDate,
            string baseCurrency,
            bool isBold = false)
        {
            Data = winLossDetailMAg;
            DataTotal = winLossTotal;
            UplineRoleId = uplineRoleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            BaseCurrency = baseCurrency;
            IsBold = isBold;
        }

        public List<WinLossByCurrencyDetailMAg> Data { get; }

        public List<WinLossByCurrencyDetailMAg> DataTotal { get; }

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

            using (var report = new WinLossByCurrencyDetailReportMAg(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}

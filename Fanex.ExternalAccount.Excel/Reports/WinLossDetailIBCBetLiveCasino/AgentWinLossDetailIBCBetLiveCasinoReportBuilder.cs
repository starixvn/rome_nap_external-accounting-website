namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class AgentWinLossDetailIBCBetLiveCasinoReportBuilder : IWinLossDetailIBCBetLiveCasinoReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public AgentWinLossDetailIBCBetLiveCasinoReportBuilder(
            List<WinLossDetailIBCBetLiveCasinoMAg> winLossDetailIBCBetLiveCasinoMAg,
            int roleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetailIBCBetLiveCasinoMAg;
            RoleId = roleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<WinLossDetailIBCBetLiveCasinoMAg> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailIBCBetLiveCasinoPath(RoleId);
            var mem = new MemoryStream();

            using (var report = new WinLossDetailIBCBetLiveCasinoReportMAg(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}

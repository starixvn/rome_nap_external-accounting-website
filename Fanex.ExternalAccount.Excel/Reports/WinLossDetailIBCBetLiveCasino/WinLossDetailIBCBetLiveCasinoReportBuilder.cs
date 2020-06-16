namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class WinLossDetailIBCBetLiveCasinoReportBuilder : IWinLossDetailIBCBetLiveCasinoReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public WinLossDetailIBCBetLiveCasinoReportBuilder(
            List<WinLossDetailIBCBetLiveCasino> winLossDetailIBCBetLiveCasino,
            int roleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetailIBCBetLiveCasino;
            RoleId = roleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<WinLossDetailIBCBetLiveCasino> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailIBCBetLiveCasinoPath(RoleId);
            var mem = new MemoryStream();

            using (var report = new WinLossDetailIBCBetLiveCasinoReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}

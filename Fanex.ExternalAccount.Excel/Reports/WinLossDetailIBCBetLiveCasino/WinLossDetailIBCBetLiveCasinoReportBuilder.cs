namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using DTO.Report;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class WinLossDetailIBCBetLiveCasinoReportBuilder : IWinLossDetailIBCBetLiveCasinoReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public WinLossDetailIBCBetLiveCasinoReportBuilder(
            List<WinLossDetailIBCBetLiveCasino> winLossDetailIBCBetLiveCasino,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetailIBCBetLiveCasino;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<WinLossDetailIBCBetLiveCasino> Data { get; }

        public bool IsBold { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailIBCBetLiveCasinoPath();
            var memoryStream = new MemoryStream();

            using (var report = new WinLossDetailIBCBetLiveCasinoReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(memoryStream);
            }

            return memoryStream;
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class SuperWinLossDetailByMCPTReportBuilder : ISuperWinLossDetailByPTReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public SuperWinLossDetailByMCPTReportBuilder(
            List<SuperWinLossDetailMCByUsernameModel> winLossDetailByUsernames,
            List<SuperWinLossDetailMCByProductDTO> totalByCurrencies,
            string baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            WinLossDetailByUsernames = winLossDetailByUsernames;
            TotalByCurrencies = totalByCurrencies;
            BaseCurrency = baseCurrency;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<SuperWinLossDetailMCByUsernameModel> WinLossDetailByUsernames { get; }

        public List<SuperWinLossDetailMCByProductDTO> TotalByCurrencies { get; }

        public string BaseCurrency { get; }

        public bool IsBold { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.SuperWinLossDetailMCByPTPath();
            var mem = new MemoryStream();

            using (var report = new SuperWinLossDetailMCByPTReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
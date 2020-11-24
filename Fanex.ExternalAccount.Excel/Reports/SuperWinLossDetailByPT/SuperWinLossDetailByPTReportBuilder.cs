namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class SuperWinLossDetailByPTReportBuilder : ISuperWinLossDetailByPTReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public SuperWinLossDetailByPTReportBuilder(
            List<SuperWinLossDetailByUsernameModel> winLossDetailByUsernames,
            SuperWinLossDetailByProductDTO grandtotal,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            WinLossDetailByUsernames = winLossDetailByUsernames;
            Grandtotal = grandtotal;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<SuperWinLossDetailByUsernameModel> WinLossDetailByUsernames { get; }

        public SuperWinLossDetailByProductDTO Grandtotal { get; }

        public bool IsBold { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.SuperWinLossDetailByPTPath();
            var mem = new MemoryStream();

            using (var report = new SuperWinLossDetailByPTReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.WinLossDetail;
    using System;
    using System.IO;

    public class CashFeeBuilder : ICashFeeBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public CashFeeBuilder(
            CashFee data,
            DateTime fromDate,
            DateTime toDate,
            string userName,
            bool isBold = false)
        {
            Data = data;
            UserName = userName;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public CashFee Data { get; }

        public bool IsBold { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public string UserName { get; }

        public MemoryStream Build(string products = null, string userName = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.CashFeePath();
            var mem = new MemoryStream();

            using (var report = new CashFeeReport(filePath, this, products, userName))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
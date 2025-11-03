using Fanex.ExternalAccount.DTO.Report;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    public class WinLossByCurrencyDetailReportBuilder : IWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public WinLossByCurrencyDetailReportBuilder(
            List<WinLossByCurrencyDetail> winLossDetail,
            List<WinLossByCurrencyDetail> winLossTotal,
            int uplineRoleId,
            DateTime fromDate,
            DateTime toDate,
            string baseCurrency,
            bool isBold = false)
        {
            Data = winLossDetail;
            DataTotal = winLossTotal;
            UplineRoleId = uplineRoleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            BaseCurrency = baseCurrency;
            IsBold = isBold;
        }

        public List<WinLossByCurrencyDetail> Data { get; }
        public List<WinLossByCurrencyDetail> DataTotal { get; }

        public bool IsBold { get; }

        public int UplineRoleId { get; }
        
        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public string BaseCurrency { get; }

        // public MemoryStream Build(string products = null)
        // {
        //     var settings = new TemplateSettings();
        //     var filePath = settings.WinLossDetailPath(RoleId);
        //     var mem = new MemoryStream();
        //
        //     using (var report = new WinLossByCurrencyDetailReport(filePath, this, products))
        //     {
        //         var hssfWorkbook = report.Build();
        //         hssfWorkbook.Write(mem);
        //     }
        //
        //     return mem;
        // }

        MemoryStream IWinLossDetailReportBuilder.Build(string products)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossByCurrencyDetailPath(UplineRoleId);
            var mem = new MemoryStream();

            using (var report = new WinLossByCurrencyDetailReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}

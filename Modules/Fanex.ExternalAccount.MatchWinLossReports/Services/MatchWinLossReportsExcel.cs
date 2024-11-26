namespace Fanex.ExternalAccount.MatchWinLossReports.Services
{
    using System;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System.IO;
    using Models;
    using NPOI.SS.Util;
    using System.Linq;
    using System.IO;
    using System.Web;
    using System.Globalization;

    public class MatchWinLossReportsExcel
    {
        private const string SummaryTemplatePath = "~/ExcelTemplates/Reports/MatchWinLossSummary.xls";
        private const string DetailTemplatePath = "~/ExcelTemplates/Reports/MatchWinLossDetail.xls";
        private const string ExcelFileNameDateFormat = "yyyyMMdd";
        
        private readonly DetailReportQuery detailQuery;
        private readonly SummaryReportResult summaryQuery;
        
        public MemoryStream BuildSummaryReportStream(SummaryReportResult report, SummaryReportQuery query)
        {
            var memoryStream = new MemoryStream();

            using (var excel = new SummaryReportExcel(SummaryReportTemplatePhysicalPath, report, query))
            {
                excel.Build().Write(memoryStream);
                return memoryStream;
            }
        }

        public MemoryStream BuildDetailReportStream(DetailReportResult report, DetailReportQuery query)
        {
            var memoryStream = new MemoryStream();

            using (var excel = new DetailReportExcel(DetailReportTemplatePhysicalPath, report, query))
            {
                excel.Build().Write(memoryStream);
                return memoryStream;
            }
        }

        private static string SummaryReportTemplatePhysicalPath =>
            HttpContext.Current.Server.MapPath(SummaryTemplatePath);

        private static string DetailReportTemplatePhysicalPath =>
            HttpContext.Current.Server.MapPath(DetailTemplatePath);

        public static string FormatDateWithoutSeconds(string dateString)
        {
            string inputFormat = "M/d/yyyy h:mm:ss tt";

            DateTime parsedDate = DateTime.ParseExact(dateString, inputFormat, CultureInfo.InvariantCulture);

            // Define the output format without seconds
            string outputFormat = "MM/dd/yyyy h:mm tt";

            // Return the formatted string
            return parsedDate.ToString(outputFormat);
        }

        public string GetDetailReportFileName(DateTime fromDate) =>
            string.Format("MatchWinLoss_Detail_{0}.xls", fromDate.ToString(ExcelFileNameDateFormat));

        public string GetSummaryReportFileName(DateTime fromDate, DateTime toDate) =>
            string.Format("MatchWinLoss_Summary_{0}_{1}.xls", fromDate.ToString(ExcelFileNameDateFormat), toDate.ToString(ExcelFileNameDateFormat));
    }
}
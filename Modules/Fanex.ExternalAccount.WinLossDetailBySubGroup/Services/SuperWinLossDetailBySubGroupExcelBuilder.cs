namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services
{
    using System;
    using System.IO;
    using System.Web;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    using Fanex.ExternalAccount.Utilities;

    public interface ISuperWinLossDetailBySubGroupExcelBuilder
    {
        MemoryStream BuildMemoryStream();

        string GetExcelFileName();
    }

    public class SuperWinLossDetailBySubGroupExcelBuilder : ISuperWinLossDetailBySubGroupExcelBuilder
    {
        private const string TemplatePath = "~/ExcelTemplates/Reports/SuperWinLossDetailBySubGroup.xls";
        private const string ExcelFileName = "SuperWinLossDetailBySubGroup";
        private const string ExcelFileNameDateFormat = "yyyyMMdd";

        private readonly string productNames;
        private readonly SuperWinLossDetailBySubGroupReport report;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private readonly string subGroup;

        public SuperWinLossDetailBySubGroupExcelBuilder(
            string productNames,
            SuperWinLossDetailBySubGroupReport report,
            DateTime fromDate,
            DateTime toDate,
            string subGroup)
        {
            this.productNames = productNames;
            this.report = report;
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.subGroup = subGroup;
        }

        public MemoryStream BuildMemoryStream()
        {
            var memoryStream = new MemoryStream();

            using (var excel = new SuperWinLossDetailBySubGroupTemplateReport(
                PhysicalTemplatePath,
                productNames,
                report,
                fromDate,
                toDate,
                subGroup))
            {
                excel.Build().Write(memoryStream);

                return memoryStream;
            }
        }

        private static string PhysicalTemplatePath => HttpContext.Current.Server.MapPath(TemplatePath);

        public string GetExcelFileName()
            => string.Format(
                "{0}_{1}_{2}.xls",
                ExcelFileName,
                fromDate.ToStringExt(ExcelFileNameDateFormat),
                toDate.ToStringExt(ExcelFileNameDateFormat));
    }
}

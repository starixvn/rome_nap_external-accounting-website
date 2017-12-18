namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class WinLossDetailReportBuilder
    {
        public WinLossDetailReportBuilder(
            List<WinLossDetail> winLossDetail,
            bool isBold,
            int roleId,
            string fromDate,
            string toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
            IsBold = isBold;
            RoleId = roleId;
            Data = winLossDetail;
        }

        public List<WinLossDetail> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build(string products = null)
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailPath(RoleId);
            var mem = new MemoryStream();

            using (var report = new WinLossDetailReport(filePath, this, products))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
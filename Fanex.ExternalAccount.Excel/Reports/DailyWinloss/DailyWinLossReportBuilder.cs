namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Collections.Generic;
    using System.IO;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Report;
    using NPOI.HSSF.UserModel;

    public class DailyWinLossReportBuilder
    {
        public DailyWinLossReportBuilder(List<DailyWinLoss> dailyWinLoss, bool isBold, int roleId, string winlostDate)
        {
            this.WinlostDate = winlostDate;
            this.IsBold = isBold;
            this.RoleId = roleId;
            this.Data = dailyWinLoss;
        }

        public List<DailyWinLoss> Data { get; private set; }

        public bool IsBold { get; private set; }

        public int RoleId { get; private set; }

        public string WinlostDate { get; private set; }

        public MemoryStream Build()
        {
            TemplateReport report = null;
            TemplateSettings settings = new TemplateSettings();
            string filePath = settings.DailyWinLossPath();

            report = new DailyWinLossReport(filePath, this);

            MemoryStream mem = new MemoryStream();
            HSSFWorkbook hssfWorkbook = ((IReportBuilder)report).Build();
            if (hssfWorkbook != null)
            {
                hssfWorkbook.Write(mem);
                return mem;
            }

            report.Dispose();

            return null;
        }
    }
}
using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class DailyWinLossReportBuilder
    {
        public DailyWinLossReportBuilder(List<DailyWinLoss> dailyWinLoss, bool isBold, int roleId, string winlostDate)
        {
            WinlostDate = winlostDate;
            IsBold = isBold;
            RoleId = roleId;
            Data = dailyWinLoss;
        }

        public List<DailyWinLoss> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string WinlostDate { get; }

        public MemoryStream Build()
        {
            var settings = new TemplateSettings();
            var filePath = "";
            switch (RoleId)
            {
                case Role.Super:
                    filePath = settings.MasterDailyWinLossPath();
                    break;
                case Role.Master:
                    filePath = settings.AgentDailyWinLossPath();
                    break;
                default:
                    filePath = settings.DailyWinLossPath();
                    break;
            }
            var mem = new MemoryStream();

            using (var report = new DailyWinLossReport(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
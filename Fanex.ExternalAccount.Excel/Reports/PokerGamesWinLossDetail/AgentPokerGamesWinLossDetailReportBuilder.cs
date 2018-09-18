namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DTO.Report;

    public class AgentPokerGamesWinLossDetailReportBuilder : IPokerGamesWinLossDetailReportBuilder
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";

        public AgentPokerGamesWinLossDetailReportBuilder(
            List<PokerGamesWinLossDetailMAg> winLossDetailMAg,
            int roleId,
            DateTime fromDate,
            DateTime toDate,
            bool isBold = false)
        {
            Data = winLossDetailMAg;
            RoleId = roleId;
            FromDate = fromDate.ToString(MonthDayYearFormat);
            ToDate = toDate.ToString(MonthDayYearFormat);
            IsBold = isBold;
        }

        public List<PokerGamesWinLossDetailMAg> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build()
        {
            var settings = new TemplateSettings();
            var filePath = settings.WinLossDetailPath(RoleId);
            var mem = new MemoryStream();

            using (var report = new PokerGamesWinLossDetailReportMAg(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}
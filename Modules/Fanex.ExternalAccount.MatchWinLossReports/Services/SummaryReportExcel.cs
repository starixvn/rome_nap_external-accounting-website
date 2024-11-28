namespace Fanex.ExternalAccount.MatchWinLossReports.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.Excel;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.MatchWinLossReports.Models;
    using Fanex.ExternalAccount.Excel.Utility;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class SummaryReportExcel : TemplateReport, IReportBuilder
    {
        private readonly SummaryReportResult report;
        private readonly SummaryReportQuery query;
        private const int LastColumnIndex = 20;
        private const int WhiteRowTemplateIndex = 3;
        private const int TotalRowTemplateIndex = 5;
        private const int SystemParlayRowTemplateIndex = 7;
        private const int LeagueNameRowTemplateIndex = 9;
        public SummaryReportExcel(
            string filePath,
            SummaryReportResult report,
            SummaryReportQuery query
        ) : base(filePath)
        {
            this.report = report;
            this.query = query;
        }

        public HSSFWorkbook Build()
        {
            var reportSheet = HssfWorkbook.GetSheet("MatchWinLoss");

            BuildMainTitle(reportSheet);
            WriteBodyReport(reportSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            const string dateFormat = "MMM dd, yyyy";
            var subTitle = "";

            if (query.IsMY)
            {
                subTitle += $", Currency: {query.Currencies ?? "All"}";
                subTitle += $", Base Currency: {query.BaseCurrency ?? "Customer Currency"}";
            }

            var mainTitle = this.query.FromDate.Equals(query.ToDate)
                ? $"Match Win Loss Summary on {query.FromDate.ToStringExt(dateFormat)}{subTitle}"
                : $"Match Win Loss Summary from {query.FromDate.ToStringExt(dateFormat)} to {query.ToDate.ToStringExt(dateFormat)}{subTitle}";
            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var rowIndex = 1;
            var toRowId = 3;

            foreach (var groupLeague in report.GroupLeagues )
            {
                resultSheet.CopyRow(TemplateSheet, LeagueNameRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
                var leagueName = new object[] { groupLeague.LeagueName };
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, leagueName);
                rowIndex++;
                toRowId++;
                foreach (var matchEvent in report.Events)
                {
                    if (matchEvent.LeagueId != groupLeague.LeagueId)
                    {
                        continue;
                    }
                    resultSheet.CopyRow(TemplateSheet, WhiteRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
                    var firstCell = MatchWinLossReportsExcel.FormatDateWithoutSeconds(matchEvent.KickOffTimeStr);
                    var secondCell = matchEvent.HomeName + '\n' + matchEvent.AwayName;
                
                    var cells = AddCommonCell(matchEvent, firstCell, secondCell);
                    resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, cells);
                    rowIndex++;
                    toRowId++;
                }
            }
            
           

            WriteFooter(resultSheet, ref rowIndex, ref toRowId);
        }

        private void WriteFooter(ISheet resultSheet, ref int rowIndex, ref int toRowId)
        {
            if (report.Events.Count != 0 && !report.Total.IsNullOrDBNull() && !report.Total.Equals(report.GrandTotal))
            {
                var text = query.IsMY ? $"Total in {query.BaseCurrency ?? query.Currencies}" : "Total";
                WriteTotal(resultSheet, report.Total, ref rowIndex, ref toRowId, text);
            }

            if (report.SystemParlayOutright.Count != 0)
            {
                WriteSystemParlay(resultSheet, report.SystemParlayOutright, ref rowIndex, ref toRowId);
            }

            if (!report.GrandTotal.IsNullOrDBNull())
            {
                var text = query.IsMY ? $"Grand Total in {query.BaseCurrency ?? query.Currencies}" : "Grand Total";
                WriteTotal(resultSheet, report.GrandTotal, ref rowIndex, ref toRowId, text);
            }
        }

        private void WriteTotal(ISheet resultSheet, ReportRawEntity row, ref int rowIndex, ref int toRowId, string name)
        {
            resultSheet.CopyRow(TemplateSheet, TotalRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
            var cells = AddCommonCell(row, name, name);
            resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, cells);
            rowIndex++;
            toRowId++;
        }

        private void WriteSystemParlay(ISheet resultSheet, List<ReportRawEntity> systemParlayOutright, ref int rowIndex, ref int toRowId)
        {
            foreach (var row in systemParlayOutright)
            {
                resultSheet.CopyRow(TemplateSheet, SystemParlayRowTemplateIndex, toRowId, FirstColumnIndex,
                    LastColumnIndex);
                var cells = AddCommonCell(row, row.BetTypeName, row.BetTypeName);
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, cells);
                rowIndex++;
                toRowId++;
            }
        }

        private object[] AddCommonCell(ReportRawEntity row, string firstCell, string secondCell)
        {
            return new object[]
            {
                firstCell,
                secondCell,
                row.StakeAvg,
                row.BetCount,
                row.Turnover,
                row.NetTurnover,
                row.GrossComm,
                row.MemberWinLoss,
                row.MemberComm,
                row.MemberTotal,
                row.MemberMargin,
                row.AgentWinLoss,
                row.AgentComm,
                row.AgentTotal,
                row.MasterWinLoss,
                row.MasterComm,
                row.MasterTotal,
                row.SuperWinLoss,
                row.SuperComm,
                row.SuperTotal,
                row.CompanyTotal
            };
        }
        
    }
}
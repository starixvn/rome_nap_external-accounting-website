namespace Fanex.ExternalAccount.MatchWinLossReports.Services
{
    using System.Collections.Generic;
    // using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.Excel;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.MatchWinLossReports.Models;
    using Fanex.ExternalAccount.Excel.Utility;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class DetailReportExcel : TemplateReport, IReportBuilder
    {
        private readonly DetailReportResult report;
        private readonly DetailReportQuery query;

        private const int LastColumnIndex = 21;
        private const int WhiteRowTemplateIndex = 3;
        private const int YellowRowTemplateIndex = 5;

        private const int TotalRowTemplateIndex = 7;
        private const int SystemParlayRowTemplateIndex = 9;
        
        private const int LeagueNameRowTemplateIndex = 11;

        public DetailReportExcel(string filePath, DetailReportResult report, DetailReportQuery query) : base(filePath)
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

            var mainTitle = $"Match Win Loss Detail on {query.FromDate.ToStringExt(dateFormat)}{subTitle}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var rowIndex = 1;
            var toRowId = 3;
            var numberRows = report.Events.Count;
            var currentMergeRowNumber = 0;
            var numberCol = 2; // KoTimeCol, EventCol
            foreach (var groupLeague in report.GroupLeagues)
            {
                resultSheet.CopyRow(TemplateSheet, LeagueNameRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
                var leagueName = new object[] { groupLeague.LeagueName };
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, leagueName);
                rowIndex++;
                toRowId++;

                for (var index = 0; index < numberRows; index++)
                {
                    if (report.Events[index].LeagueId != groupLeague.LeagueId) continue;
                    var matchEvent = report.Events[index];
                    if (currentMergeRowNumber == 0)
                    {
                        resultSheet.CopyRow(TemplateSheet, WhiteRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
                        currentMergeRowNumber = report.Events.FindAll(e => e.MatchId == matchEvent.MatchId).Count;
                        var rowData = CreateRowData(matchEvent, new List<object>
                        {
                            MatchWinLossReportsExcel.FormatDateWithoutSeconds(matchEvent.KickOffTimeStr),
                            matchEvent.HomeName + '\n' + matchEvent.AwayName,
                            matchEvent.BetTypeName,
                        });

                        resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, rowData);
                        MergedRegion(resultSheet, toRowId, toRowId + currentMergeRowNumber - 1);
                    }
                    else
                    {
                        var isSubTotal = matchEvent.BetTypeId == 0;
                        var rowTemplateIndex = isSubTotal ? YellowRowTemplateIndex : WhiteRowTemplateIndex;
                        var subTotalText = query.IsMY ? $"Sub Total in {query.BaseCurrency ?? query.Currencies}" : "Sub Total";
                        var betTypeName = isSubTotal ? subTotalText : matchEvent.BetTypeName;

                        resultSheet.CopyRow(TemplateSheet, rowTemplateIndex, toRowId, FirstColumnIndex + numberCol, LastColumnIndex);
                        var rowData = CreateRowData(matchEvent, new List<object> { "", "", betTypeName });
                        resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, rowData);
                    }

                    currentMergeRowNumber--;
                    rowIndex++;
                    toRowId++;
                }
            }

            WriteFooter(resultSheet, ref rowIndex, ref toRowId);
        }


        private void WriteFooter(ISheet resultSheet, ref int rowIndex, ref int toRowId)
        {
            if (report.Events.Count != 0 && !report.Total.Equals(null) && !report.Total.Equals(report.GrandTotal))
            {
                var text = query.IsMY ? $"Total in {query.BaseCurrency ?? query.Currencies}" : "Total";
                WriteTotal(resultSheet, report.Total, ref rowIndex, ref toRowId, text);
            }

            if (report.SystemParlayOutright.Count != 0)
            {
                WriteSystemParlay(resultSheet, report.SystemParlayOutright, ref rowIndex, ref toRowId);
            }

            if (!report.GrandTotal.Equals(null))
            {
                var text = query.IsMY ? $"Grand Total in {query.BaseCurrency ?? query.Currencies}" : "Grand Total";
                WriteTotal(resultSheet, report.GrandTotal, ref rowIndex, ref toRowId, text);
            }
        }

        private void WriteTotal(ISheet resultSheet, ReportRawEntity total, ref int rowIndex, ref int toRowId,
            string name)
        {
            resultSheet.CopyRow(TemplateSheet, TotalRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);
            var rowData = CreateRowData(total, new List<object> { name, name, name });

            resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, rowData);
            rowIndex++;
            toRowId++;
        }

        private void WriteSystemParlay(ISheet resultSheet, List<ReportRawEntity> systemParlayOutright, ref int rowIndex,
            ref int toRowId)
        {
            foreach (var row in systemParlayOutright)
            {
                resultSheet.CopyRow(TemplateSheet, SystemParlayRowTemplateIndex, toRowId, FirstColumnIndex,
                    LastColumnIndex);
                var rowData = CreateRowData(row, new List<object>
                {
                    row.BetTypeName,
                    row.BetTypeName,
                    row.BetTypeName
                });
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, rowData);
                rowIndex++;
                toRowId++;
            }
        }

        private object[] CreateRowData(ReportRawEntity row, List<object> dynamicValues = null)
        {
            var rowData = new List<object>();

            if (dynamicValues != null)
            {
                rowData.AddRange(dynamicValues);
            }

            rowData.AddRange(new object[]
            {
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
            });

            return rowData.ToArray();
        }

        private void MergedRegion(ISheet worksheet, int firstCellIndex, int lastCellIndex)
        {
            if (lastCellIndex <= firstCellIndex) return;
            var mergedNumberCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 0, 0);
            var mergedHeadlineCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 1, 1);
            worksheet.AddMergedRegion(mergedNumberCol);
            worksheet.AddMergedRegion(mergedHeadlineCol);
        }
    }
}
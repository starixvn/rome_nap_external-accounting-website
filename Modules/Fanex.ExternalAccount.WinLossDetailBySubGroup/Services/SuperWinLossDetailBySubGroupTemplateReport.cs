namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.Excel;
    using Fanex.ExternalAccount.Excel.Utility;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    using Fanex.ExternalAccount.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class SuperWinLossDetailBySubGroupTemplateReport : TemplateReport, IReportBuilder
    {
        private const int LastColumnIndex = 17;
        private const int ParentRowTemplateIndex = 1;
        private const int ChildRowTemplateIndex = 3;
        private readonly SuperWinLossDetailBySubGroupReport report;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private readonly string subGroup;

        public SuperWinLossDetailBySubGroupTemplateReport(
            string filePath,
            string products,
            SuperWinLossDetailBySubGroupReport report,
            DateTime fromDate,
            DateTime toDate,
            string subGroup) : base(filePath, products)
        {
            this.report = report;
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.subGroup = subGroup;
        }

        public HSSFWorkbook Build()
        {
            var reportSheet = HssfWorkbook.GetSheet("SuperWinLossDetailBySubGroup");

            BuildMainTitle(reportSheet);
            if(report.SuperWinLossDetails.Any()) CreateRewardCell(reportSheet, LastColumnIndex);
            WriteBodyReport(reportSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            const string DateFormat = "MMM dd, yyyy";

            var mainTitle = new StringBuilder();
            mainTitle.Append("Super Win Loss Detail By Sub Group; ");

            if (fromDate == toDate)
            {
                mainTitle
                    .Append("On ")
                    .Append(fromDate.ToStringExt(DateFormat));
            }
            else
            {
                mainTitle
                    .Append(fromDate.ToStringExt(DateFormat))
                   .Append(" To ")
                   .Append(toDate.ToStringExt(DateFormat));
            }

            if (!string.IsNullOrWhiteSpace(subGroup))
            {
                mainTitle
                    .Append("; ")
                    .Append(subGroup);
            }

            mainTitle
                .Append("; ")
                .Append(Products);

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle.ToString());
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var rowIndex = 1;
            var toRowId = 3;

            foreach (var winlossBySubGroup in report.SuperWinLossDetails)
            {
                resultSheet.CopyRow(TemplateSheet, ParentRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);

                var parentRowValues = new object[]
                {
                    winlossBySubGroup.SubGroup,
                    winlossBySubGroup.Turnover,
                    winlossBySubGroup.NetTurnover,
                    winlossBySubGroup.GrossComm,
                    winlossBySubGroup.MemberWinLoss,
                    winlossBySubGroup.MemberComm,
                    winlossBySubGroup.MemberTotal,
                    winlossBySubGroup.AgentWinLoss,
                    winlossBySubGroup.AgentComm,
                    winlossBySubGroup.AgentTotal,
                    winlossBySubGroup.MasterWinLoss,
                    winlossBySubGroup.MasterComm,
                    winlossBySubGroup.MasterTotal,
                    winlossBySubGroup.SuperWinLoss,
                    winlossBySubGroup.SuperComm,
                    winlossBySubGroup.SuperTotal,
                    winlossBySubGroup.Company,
                    winlossBySubGroup.RewardAmount
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, parentRowValues);
                rowIndex++;
                toRowId++;

                foreach (var winloss in winlossBySubGroup.SuperWinLossDetails)
                {
                    resultSheet.CopyRow(TemplateSheet, ChildRowTemplateIndex, toRowId, FirstColumnIndex, LastColumnIndex);

                    var childRowValues = new object[]
                    {
                        winloss.Username,
                        winloss.Turnover,
                        winloss.NetTurnover,
                        winloss.GrossComm,
                        winloss.MemberWinLoss,
                        winloss.MemberComm,
                        winloss.MemberTotal,
                        winloss.AgentWinLoss,
                        winloss.AgentComm,
                        winloss.AgentTotal,
                        winloss.MasterWinLoss,
                        winloss.MasterComm,
                        winloss.MasterTotal,
                        winloss.SuperWinLoss,
                        winloss.SuperComm,
                        winloss.SuperTotal,
                        winloss.Company,
                        winloss.RewardAmount
                    };

                    resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, childRowValues);
                    rowIndex++;
                    toRowId++;
                }
            }

            WriteGrandTotal(resultSheet, report.Total, toRowId);
        }

        private void WriteGrandTotal(ISheet resultSheet, SuperWinLossDetailBySubGroupModel total, int totalRowIndex)
        {
            resultSheet.CopyRow(
                TemplateSheet,
                FirstColumnTotalIndex,
                totalRowIndex,
                FirstColumnIndex,
                LastColumnIndex);

            var totalRowValues = new object[]
            {
                "Grand Total",
                total.Turnover,
                total.NetTurnover,
                total.GrossComm,
                total.MemberWinLoss,
                total.MemberComm,
                total.MemberTotal,
                total.AgentWinLoss,
                total.AgentComm,
                total.AgentTotal,
                total.MasterWinLoss,
                total.MasterComm,
                total.MasterTotal,
                total.SuperWinLoss,
                total.SuperComm,
                total.SuperTotal,
                total.Company,
                total.RewardAmount
            };

            resultSheet.GetRow(totalRowIndex).WriteCells(FirstColumnIndex, totalRowValues);
        }

        private void CreateRewardCell(ISheet resultSheet, int rewardColumnIndex)
        {
            var headerRow = resultSheet.GetRow(1);
            var currentCell = headerRow.GetCell(rewardColumnIndex);
            var valueCell = $"Reward ({report.SuperWinLossDetails?.ElementAt(0)?.RewardCurrency ?? ""})";
            currentCell.SetCellValue(valueCell);
        }
    }
}

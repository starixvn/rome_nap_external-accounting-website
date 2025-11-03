namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class WinLossDetailReportMAg : TemplateReport, IReportBuilder
    {
        private readonly AgentWinLossDetailReportBuilder _builder;
        private const int LastColumnIndex = 15;

        public WinLossDetailReportMAg(string filePath, AgentWinLossDetailReportBuilder builder, string products = null)
            : base(filePath, products)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("WinLossDetailMAg");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            CreateRewardCell(resultSheet, LastColumnIndex);
            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);
            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var index = 1;
            var toRowId = 3;

            foreach (var item in _builder.Data)
            {
                var fromRowId = index % 2 == 0 ? OddRowIndex : EvenRowIndex;
                resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, LastColumnIndex);

                var values = new List<object>
                {
                    item.Username ?? "Dormant Accounts",
                    TemplateSettings.GetLevelType(item.RoleId),
                    item.Turnover,
                    item.NetTurnover,
                    item.GrossComm,
                    item.MemberWinlost,
                    item.PlayerComm,
                    item.MemberTotal,
                    item.AgWinlost,
                    item.AgComm,
                    item.AgTotal,
                    item.MAgWinlost,
                    item.MAgComm,
                    item.MAgTotal,
                    item.Company,
                    item.RewardAmount
                };

                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, values);
                index++;
                toRowId++;
            }

            WriteTotal(resultSheet, toRowId);
        }

        private void WriteTotal(ISheet resultSheet, int toRowIndex)
        {
            var data = _builder.Data;
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, LastColumnIndex);
            var values = new List<object>
            {
                "Grand Total",
                string.Empty,
                data.Sum(p => p.Turnover),
                data.Sum(p => p.NetTurnover),
                data.Sum(p => p.GrossComm),
                data.Sum(p => p.MemberWinlost),
                data.Sum(p => p.PlayerComm),
                data.Sum(p => p.MemberTotal),
                data.Sum(p => p.AgWinlost),
                data.Sum(p => p.AgComm),
                data.Sum(p => p.AgTotal),
                data.Sum(p => p.MAgWinlost),
                data.Sum(p => p.MAgComm),
                data.Sum(p => p.MAgTotal),
                data.Sum(p => p.Company),
                data.Sum(p => p.RewardAmount)
            };

            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, values);
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.FromDate.Equals(_builder.ToDate)
                ? $"Agent Win Loss Detail on {_builder.FromDate}"
                : $"Agent Win Loss Detail From {_builder.FromDate} To {_builder.ToDate}";

            mainTitle += string.IsNullOrEmpty(Products) ? string.Empty : $" - {Products}";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
        
        private void CreateRewardCell(ISheet resultSheet,int rewardColumnIndex)
        {
            var headerRow = resultSheet.GetRow(1);
            var currentCell = headerRow.GetCell(rewardColumnIndex);
            var valueCell = $"Reward ({_builder.Data[0].RewardCurrency ?? ""})";
            currentCell.SetCellValue(valueCell);
        }
    }
}
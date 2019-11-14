namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class CustomerListPage : TemplateReport, IReportBuilder
    {
        private readonly CustomerListPageBuilder _builder;
        public CustomerListPage(string filePath, CustomerListPageBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var resultSheet = HssfWorkbook.GetSheet("CustomerList");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var dtr = _builder.Data.Rows;
            var totalColumn = _builder.RoleId.IsSuper() ? 15 : 14;
            var lastColumnIndex = FirstColumnIndex + totalColumn - 1;
            if (_builder.RoleId.IsNotMember())
            {
                WriteOthersReport(resultSheet, dtr, lastColumnIndex);
            }
            else
            {
                totalColumn = 18;
                lastColumnIndex = FirstColumnIndex + totalColumn - 1;
                WriteMemberReport(resultSheet, dtr, lastColumnIndex);
            }
        }

        private void WriteMemberReport(
            ISheet resultSheet,
            DataRowCollection dtr,
            int lastColumnIndex)
        {
            var id = 1;
            var rowId = 3;
            foreach (DataRow items in dtr)
            {
                var custStatus = SetBackgroundRow(resultSheet, rowId, lastColumnIndex, items);

                var values = new List<object>
                {
                    id,
                    items.Username(),
                    custStatus,
                    items.DoubleCommLosing() ? "ON" : "OFF",
                    items.NickName(),
                    items.FirstName(),
                    items.LastName(),
                    items.UserGroup(),
                    items.Discount(),
                    items.Discount1x2(),
                    items.DiscountCasino(),
                    items.PlayerDiscount(),
                    items.PlayerDiscount1x2(),
                    items.PlayerDiscountCasino(),
                    items.CreateDate(),
                    items.LastLoginIP()
                };

                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);

                id++;
                rowId++;
            }
        }

        private void WriteOthersReport(
            ISheet resultSheet,
            DataRowCollection dtr,
            int lastColumnIndex)
        {
            var id = 1;
            var rowId = 3;
            foreach (DataRow items in dtr)
            {
                var custStatus = SetBackgroundRow(resultSheet, rowId, lastColumnIndex, items);
                var values = new List<object>
                {
                    id,
                    items.Username(),
                    custStatus,
                    items.DoubleCommLosing() ? "ALLOWED" : "DISALLOWED",
                    items.FirstName(),
                    items.LastName(),
                    items.GroupA(),
                    items.GroupB(),
                    items.GroupC(),
                    items.GroupD(),
                    items.Discount1x2(),
                    items.DiscountCasino()
                };

                if (_builder.RoleId.IsSuper())
                {
                    values.Add(items.LimitDays()?.ToString() ?? items.MinDate() ?? string.Empty);
                }

                values.Add(items.CreateDate());
                values.Add(items.LastLoginIP());
                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);
                id++;
                rowId++;
            }
        }

        private string SetBackgroundRow(
            ISheet resultSheet,
            int rowId,
            int lastColumnIndex,
            DataRow items)
        {
            var custStatus = !items.Enabled() ? "disabled"
                : items.Closed() ? "closed"
                : items.Suspended() ? "suspended"
                : "open";

            var rowIdFromStatus = new Dictionary<string, int> { { "open", 1 }, { "suspended", 3 }, { "disabled", 7 } };
            var rowIdFromCloseStatus = new Dictionary<byte, int> { { 1, 11 }, { 2, 13 }, { 3, 15 } };
            var closedReason = items.ClosedReason();

            var fromRowId = 5;
            if (rowIdFromStatus.ContainsKey(custStatus))
            {
                fromRowId = rowIdFromStatus[custStatus];
            }
            {
                fromRowId = rowIdFromCloseStatus.ContainsKey(closedReason) ? rowIdFromCloseStatus[closedReason] : fromRowId;
            }

            resultSheet.CopyRow(TemplateSheet, fromRowId, rowId, FirstColumnIndex, lastColumnIndex);

            return custStatus;
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = $"{_builder.Status} {_builder.RoleId.DisplayRoleName()} List";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
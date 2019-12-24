namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using Core.Shared;
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
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
            var totalColumn = _builder.RoleId.IsSuper() ? 15 : 14;
            var lastColumnIndex = FirstColumnIndex + totalColumn - 1;
            if (_builder.RoleId.IsNotMember())
            {
                WriteOthersReport(resultSheet, _builder.Data, lastColumnIndex);
            }
            else
            {
#pragma warning disable S109 // Magic numbers should not be used
                totalColumn = 18;
#pragma warning restore S109 // Magic numbers should not be used
                lastColumnIndex = FirstColumnIndex + totalColumn - 1;
                WriteMemberReport(resultSheet, _builder.Data, lastColumnIndex);
            }
        }

        private void WriteMemberReport(
            ISheet resultSheet,
            List<CustomerListDTO> data,
            int lastColumnIndex)
        {
            var id = 1;
            var rowId = 3;
            foreach (CustomerListDTO item in data)
            {
                var custStatus = SetBackgroundRow(resultSheet, rowId, lastColumnIndex, item);

                var values = new List<object>
                {
                    id,
                    item.UserName,
                    custStatus,
                    item.DoubleCommLosing ? "ON" : "OFF",
                    item.NickName,
                    item.FirstName,
                    item.LastName,
                    item.UserGroup,
                    item.Discount,
                    item.Discount1x2,
                    item.DiscountCS,
                    item.PlayerDiscount,
                    item.PlayerDiscount1x2,
                    item.PlayerDiscountCS,
                    item.CreateDate,
                    item.LastLoginIP
                };

                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);

                id++;
                rowId++;
            }
        }

        private void WriteOthersReport(
            ISheet resultSheet,
            List<CustomerListDTO> data,
            int lastColumnIndex)
        {
            var id = 1;
            var rowId = 3;
            foreach (CustomerListDTO item in data)
            {
                var custStatus = SetBackgroundRow(resultSheet, rowId, lastColumnIndex, item);
                var values = new List<object>
                {
                    id,
                    item.UserName,
                    custStatus,
                    item.DoubleCommLosing ? "ALLOWED" : "DISALLOWED",
                    item.FirstName,
                    item.LastName,
                    item.GroupA,
                    item.GroupB,
                    item.GroupC,
                    item.GroupD,
                    item.Discount1x2,
                    item.DiscountCS
                };

                if (_builder.RoleId.IsSuper())
                {
                    values.Add(item.LimitDays?.ToString() ?? item.MinDate);
                }

                values.Add(item.CreateDate);
                values.Add(item.LastLoginIP);
                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);
                id++;
                rowId++;
            }
        }

        private string SetBackgroundRow(
            ISheet resultSheet,
            int rowId,
            int lastColumnIndex,
            CustomerListDTO item)
        {
            var custStatus = !item.Enabled ? "disabled"
#pragma warning disable S3358 // Ternary operators should not be nested
                : item.Closed ? "closed"
                : item.Suspended ? "suspended"
                : "open";
#pragma warning restore S3358 // Ternary operators should not be nested

            var rowIdFromStatus = new Dictionary<string, int> { { "open", 1 }, { "suspended", 3 }, { "disabled", 7 } };
            var rowIdFromCloseStatus = new Dictionary<byte, int> { { 1, 11 }, { 2, 13 }, { 3, 15 } };
            var closedReason = item.ClosedReason;

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
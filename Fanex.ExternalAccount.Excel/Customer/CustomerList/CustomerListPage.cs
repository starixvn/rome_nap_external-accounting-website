namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Collections.Generic;
    using Core.Shared;
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    internal class CustomerListPage : TemplateReport, IReportBuilder
    {
        private const int OpenRowIndex = 1;
        private const int SuspendedRowIndex = 3;
        private const int DisabledRowIndex = 7;
        private static readonly IDictionary<byte, int> RowIndexClosedStatusMapping = new Dictionary<byte, int>
        {
            [1] = 11,
            [2] = 13,
            [4] = 17
        };
        private static readonly IDictionary<byte, int> RowIndexLockedStatusMapping = new Dictionary<byte, int>
        {
            [1] = 15,
            [2] = 9
        };

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
            
            if (_builder.IsDirectMember)
            {
                BuildDirectMemberMainTitle(resultSheet);
                WriteDirectMemberReport(resultSheet);
            }
            else
            {    
                BuildMainTitle(resultSheet);
                WriteBodyReport(resultSheet);
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public override void WriteBodyReport(ISheet resultSheet)
        {
            var totalColumn = _builder.RoleId.IsSuper() ? 17 : 16;

            if (_builder.RoleId.IsSuper())
            {
                totalColumn++;

                if (_builder.HasSubGroups)
                {
                    totalColumn++;
                }
            }

            if (_builder.IsMultipleCurrencyGroup)
            {
                totalColumn++;
            }

            var lastColumnIndex = FirstColumnIndex + totalColumn - 1;
            if (_builder.RoleId.IsNotMember())
            {
                WriteOthersReport(resultSheet, _builder.Data, lastColumnIndex, _builder.HasSubGroups, _builder.IsMultipleCurrencyGroup);
            }
            else
            {
                totalColumn = _builder.IsMultipleCurrencyGroup ? 20 : 19;
                lastColumnIndex = FirstColumnIndex + totalColumn - 1;
                WriteMemberReport(resultSheet, _builder.Data, lastColumnIndex, _builder.IsMultipleCurrencyGroup);
            }
        }

        public void WriteDirectMemberReport(ISheet resultSheet)
        {
            var totalColumn = _builder.IsMultipleCurrencyGroup ? 20 : 19;
            var lastColumnIndex = FirstColumnIndex + totalColumn - 1;
            WriteMemberReport(resultSheet, _builder.Data, lastColumnIndex, _builder.IsMultipleCurrencyGroup);
        }
        

        private void WriteMemberReport(
            ISheet resultSheet,
            List<CustomerListDTO> data,
            int lastColumnIndex,
            bool isMultipleCurrency)
        {
            var id = 1;
            var rowId = 3;
            foreach (CustomerListDTO item in data)
            {
                SetBackgroundRow(resultSheet, rowId, lastColumnIndex, item);

                var values = new List<object>
                {
                    id,
                    item.UserName,
                    item.LockedAccess.IsLocked ? "Yes" : "No",
                    GetCustomerStatus(item),
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

                if (isMultipleCurrency)
                {
                    values.Insert(2, item.Currency);
                }

                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);

                id++;
                rowId++;
            }
        }

        private void WriteOthersReport(
            ISheet resultSheet,
            List<CustomerListDTO> data,
            int lastColumnIndex,
            bool hasSubGroups,
            bool isMultipleCurrency)
        {
            var id = 1;
            var rowId = 3;
            foreach (CustomerListDTO item in data)
            {
                SetBackgroundRow(resultSheet, rowId, lastColumnIndex, item);
                var custStatus = GetCustomerStatus(item);

                var values = new List<object>
                {
                    id,
                    item.UserName,
                    item.LockedAccess.IsLocked ? "Yes" : "No",
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
               
                if (isMultipleCurrency)
                {
                    values.Insert(2, item.Currency);
                }
                
                if (_builder.RoleId.IsSuper())
                {
                    if (hasSubGroups)
                    {
                        values.Add(item.SubGroup);
                    }
                    values.Add(item.NoOfDownlines);
                    values.Add(string.IsNullOrEmpty(item.MinDate) ? item.LimitDays?.ToString() : item.MinDate);
                }
                else
                {
                    values.Add(item.NoOfDownlines);
                }

                values.Add(item.CreateDate);
                values.Add(item.LastLoginIP);

                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);
                id++;
                rowId++;
            }
        }

        private void SetBackgroundRow(
            ISheet resultSheet,
            int rowId,
            int lastColumnIndex,
            CustomerListDTO item)
        {
            var fromRowId = MapRowIdByCustomerStatus(item);

            resultSheet.CopyRow(TemplateSheet, fromRowId, rowId, FirstColumnIndex, lastColumnIndex);
        }

        private static int MapRowIdByCustomerStatus(CustomerListDTO item)
        {
            if (!item.Enabled)
            {
                return DisabledRowIndex;
            }

            if (item.Closed)
            {
                return MapRowIdByDictionaryStatus(item.ClosedReason, RowIndexClosedStatusMapping);
            }

            if (item.Suspended)
            {
                return SuspendedRowIndex;
            }

            if (item.LockedAccess.IsLocked)
            {
                return MapRowIdByDictionaryStatus(item.LockedAccess.Locked, RowIndexLockedStatusMapping);
            }

            return OpenRowIndex;
        }

        private static int MapRowIdByDictionaryStatus(byte key, IDictionary<byte, int> status)
            => status.TryGetValue(key, out var rowId) ? rowId : OpenRowIndex;

        private static string GetCustomerStatus(CustomerListDTO item)
        {
            if (!item.Enabled)
            {
                return "Disabled";
            }

            if (item.Closed)
            {
                return "Closed";
            }

            return item.Suspended ? "Suspended" : "Open";
        }

        public override void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = $"{_builder.Status} {_builder.RoleId.DisplayRoleName()} List";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }

        public void BuildDirectMemberMainTitle(ISheet resultSheet)
        {
            var mainTitle = $"{_builder.Status} {_builder.UplineRoleId.DisplayRoleName()} Direct Member List";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
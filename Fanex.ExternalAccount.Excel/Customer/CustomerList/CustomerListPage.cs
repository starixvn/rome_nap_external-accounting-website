namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utilities;
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

        private void WriteBodyReport(ISheet resultSheet)
        {
            var dtr = _builder.Data.Rows;
            var totalColumn = _builder.IsMyanmarOdds || _builder.RoleId == 4 ? 16 : 15;
            var lastColumnIndex = FirstColumnIndex + totalColumn - 1;
            if (_builder.RoleId != 1)
            {
                WriteOthersReport(resultSheet, dtr, lastColumnIndex);
            }
            else
            {
                totalColumn = 20;
                lastColumnIndex = FirstColumnIndex + totalColumn - 1;
                WriteMemberReport(resultSheet, dtr, lastColumnIndex, _builder.IsMyanmarOdds);
            }

        }

        private void WriteMemberReport(
            ISheet resultSheet,
            DataRowCollection dtr,
            int lastColumnIndex,
            bool isMyanmarOdds = false)
        {
            var id = 1;
            var rowId = 3;
            foreach (DataRow items in dtr)
            {
                var custStatus = SetBackgroundRow(resultSheet, rowId, lastColumnIndex, items);

                var values = new List<object>
                {
                    id,
                    items["username"].ToString(),
                    custStatus,
                    Utils.ToBoolean(items["DoubleCommLosing"]) ? "ON" : "OFF",
                    items["nickname"].ToString(),
                    items["firstname"].ToString(),
                    items["lastname"].ToString(),
                    items["ugroup"].ToString(),
                    Utils.ToDouble(items["discount"]),
                    Utils.ToDouble(items["discount1x2"]),
                    Utils.ToDouble(items["discountcs"]),
                    Utils.ToDouble(items["discountnumber"]),
                    Utils.ToDouble(items["playerdiscount"]),
                    Utils.ToDouble(items["playerdiscount1x2"]),
                    Utils.ToDouble(items["playerdiscountcs"]),
                    Utils.ToDouble(items["playerdiscountnumber"]),
                    $"{items["createdate"]:MM/dd/yyyy HH:mm:ss tt}",
                    items["lastloginip"].ToString()
                };

                if (isMyanmarOdds)
                {
                    values.Insert(12,
                        Utils.ToBoolean(items["ismyanmarodds"])
                            ? Utils.ToDouble(items["discountmyanmarodds"]).ToString(CultureInfo.InvariantCulture)
                            : "N/A");
                    values.Insert(17,
                        Utils.ToBoolean(items["ismyanmarodds"])
                            ? Utils.ToDouble(items["playerdiscountmyanmarodds"]).ToString(CultureInfo.InvariantCulture)
                            : "N/A");
                }

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
                    items["username"].ToString(),
                    custStatus,
                    Utils.ToBoolean(items["DoubleCommLosing"]) ? "ALLOWED" : "DISALLOWED",
                    items["firstname"].ToString(),
                    items["lastname"].ToString(),
                    Utils.ToDouble(items["groupa"]),
                    Utils.ToDouble(items["groupb"]),
                    Utils.ToDouble(items["groupc"]),
                    Utils.ToDouble(items["groupd"]),
                    Utils.ToDouble(items["discount1x2"]),
                    Utils.ToDouble(items["discountcs"]),
                    Utils.ToDouble(items["discountnumber"]),
                    Utils.ToBoolean(items["ismyanmarodds"]) ? Utils.ToDouble(items["discountmyanmarodds"]).ToString(CultureInfo.InvariantCulture) : "N/A",
                    $"{items["createdate"]:MM/dd/yyyy HH:mm:ss tt}",
                    items["lastloginip"].ToString()
                };

                values = RemoveOddsColumn(values);
                resultSheet.GetRow(rowId).WriteCells(FirstColumnIndex, values);
                id++;
                rowId++;
            }
        }

        private List<object> RemoveOddsColumn(List<object> values)
        {
            var newValues = values;
            if (_builder.RoleId != 4)
            {
                newValues.RemoveAt(13);
            }

            return newValues;
        }

        private string SetBackgroundRow(
            ISheet resultSheet,
            int rowId,
            int lastColumnIndex,
            DataRow items)
        {
            var custStatus = !Utils.ToBoolean(items["enabled"])
                ? "disabled"
                : Utils.ToBoolean(items["closed"])
                    ? "closed"
                    : Utils.ToBoolean(items["suspended"])
                        ? "suspended"
                        : "open";
            var rowIdFromStatus = new Dictionary<string, int> { { "open", 1 }, { "suspended", 3 }, { "disabled", 7 } };
            var rowIdFromCloseStatus = new Dictionary<byte, int> { { 1, 11 }, { 2, 13 }, { 3, 15 } };
            var closedReason = Convert.ToByte(items["closedreason"]);

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

        private void BuildMainTitle(ISheet resultSheet)
        {
            var mainTitle = _builder.Status + (_builder.RoleId == 1 ? " Member" :
                (_builder.RoleId == 2 ? " Agent" : (_builder.RoleId == 3 ? " Master" : " Super"))) + " List";

            resultSheet.GetCell(FirstRowIndex, FirstColumnIndex).SetCellValue(mainTitle);
        }
    }
}
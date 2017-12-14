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
            var data = _builder.Data;
            if (data == null)
            {
                return null;
            }

            var templateSheet = HssfWorkbook.GetSheet("Template");
            var resultSheet = HssfWorkbook.GetSheet("CustomerList");

            if (_builder.IsBold)
            {
                BoldAllFonts();
            }

            const int rowId = 0;
            const int colId = 0;

            var mainTitle = BuildMainTitle(_builder.Status, _builder.RoleId);

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            var count = data.Rows.Count;
            if (count > 0)
            {
                var dtr = data.Rows;
                var totalColumn = _builder.IsMyanmarOdds || _builder.RoleId == 4 ? 16 : 15;
                var lastColumnIndex = colId + totalColumn - 1;
                if (_builder.RoleId != 1)
                {
                    GetOtherList(templateSheet, resultSheet, colId, dtr, lastColumnIndex);
                }
                else
                {
                    totalColumn = 20;
                    lastColumnIndex = colId + totalColumn - 1;
                    GetMemberList(templateSheet, resultSheet, colId, dtr, lastColumnIndex, _builder.IsMyanmarOdds);
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return HssfWorkbook;
        }

        private static void GetMemberList(
            ISheet templateSheet,
            ISheet resultSheet,
            int colId,
            DataRowCollection dtr,
            int lastColumnIndex,
            bool isMyanmarOdds = false)
        {
            var id = 1;
            var rowId = 3;
            foreach (DataRow items in dtr)
            {
                var custStatus = SetBackgroundRow(templateSheet, resultSheet, rowId, colId, lastColumnIndex,
                    items);

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

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }

        private void GetOtherList(
            ISheet templateSheet,
            ISheet resultSheet,
            int colId,
            DataRowCollection dtr,
            int lastColumnIndex)
        {
            var values = new List<object>();
            var id = 1;
            var rowId = 3;
            foreach (DataRow items in dtr)
            {
                var custStatus = SetBackgroundRow(templateSheet, resultSheet, rowId, colId, lastColumnIndex, items);
                if (_builder.RoleId != 1)
                {
                    if (_builder.RoleId != 4 && Utils.ToBoolean(items["ismyanmarodds"]))
                    {
                        values = new List<object>
                        {
                            id,
                            items["username"].ToString(),
                            custStatus,
                            Utils.ToBoolean(items["DoubleCommLosing"]) ? "ALLOWED" : "DISALLOWED",
                            items["firstname"].ToString(),
                            items["lastname"].ToString(),
                            Utils.ToDouble(items["groupa"]),
                            Utils.ToDouble(items["discount1x2"]),
                            Utils.ToDouble(items["discountcs"]),
                            Utils.ToDouble(items["discountnumber"]),
                            $"{items["createdate"]:MM/dd/yyyy HH:mm:ss tt}",
                            items["lastloginip"].ToString()
                        };

                        values.Insert(10,
                            Utils.ToDouble(items["discountmyanmarodds"]).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        values = new List<object>
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
                            $"{items["createdate"]:MM/dd/yyyy HH:mm:ss tt}",
                            items["lastloginip"].ToString()
                        };

                        if (_builder.RoleId == 4 || Utils.ToBoolean(items["ismyanmarodds"]))
                        {
                            values.Insert(13,
                                Utils.ToBoolean(items["ismyanmarodds"])
                                    ? Utils.ToDouble(items["discountmyanmarodds"])
                                        .ToString(CultureInfo.InvariantCulture)
                                    : "N/A");

                            if (_builder.RoleId != 4)
                            {
                                values.RemoveAt(7);
                                values.RemoveAt(8);
                                values.RemoveAt(9);
                            }
                        }
                    }
                }

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }

        private static string SetBackgroundRow(
            ISheet templateSheet,
            ISheet resultSheet,
            int rowId,
            int colId,
            int lastColumnIndex,
            DataRow items)
        {
            var custStatus = !Utils.ToBoolean(items["enabled"])
                ? "Disabled"
                : Utils.ToBoolean(items["closed"])
                    ? "Closed"
                    : Utils.ToBoolean(items["suspended"])
                        ? "Suspended"
                        : "Open";

            if (custStatus.Equals("open", StringComparison.OrdinalIgnoreCase))
            {
                resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
            }
            else if (custStatus.Equals("suspended", StringComparison.OrdinalIgnoreCase))
            {
                resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
            }
            else if (custStatus.Equals("closed", StringComparison.OrdinalIgnoreCase))
            {
                var closedReason = Convert.ToByte(items["closedreason"]);
                int templateRowIndex;

                switch (closedReason)
                {
                    case 1:
                        templateRowIndex = 11;
                        break;
                    case 2:
                        templateRowIndex = 13;
                        break;
                    case 3:
                        templateRowIndex = 15;
                        break;
                    default:
                        templateRowIndex = 5;
                        break;
                }

                resultSheet.CopyRow(templateSheet, templateRowIndex, rowId, colId, lastColumnIndex);
            }
            else
            {
                if (custStatus.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                {
                    resultSheet.CopyRow(templateSheet, 7, rowId, colId, lastColumnIndex);
                }
            }

            return custStatus;
        }

        private static string BuildMainTitle(string status, int roleId)
            => $"{status}{(roleId == 1 ? " Member" : (roleId == 2 ? " Agent" : (roleId == 3 ? " Master" : " Super")))} List";

    }
}
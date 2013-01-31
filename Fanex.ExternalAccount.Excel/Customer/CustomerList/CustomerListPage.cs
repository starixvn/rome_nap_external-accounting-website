namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System;
    using System.Collections.Generic;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;
    using Fanex.ExternalAccount.Utilities;
    using System.Data;

    internal class CustomerListPage : TemplateReport, IReportBuilder
    {
        private CustomerListPageBuilder builder;

        public CustomerListPage(string filePath, CustomerListPageBuilder builder)
            : base(filePath)
        {
            this.builder = builder;
        }

        public HSSFWorkbook Build()
        {
            var data = this.builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("CustomerList");

            // apply Bold on all fonts
            if (this.builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            // Header
            string mainTitle = builder.Status + (builder.RoleId == 1 ? "Member" : (builder.RoleId == 2 ? "Agent" : (builder.RoleId == 3 ? "Master" : "Super"))) + " List";

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 3;
            int id = 1;

            int count = data.Rows.Count;
            DataRowCollection dtr = data.Rows;

            if (count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 15;
                int lastColumnIndex = colId + totalColumn - 1;
                if (builder.RoleId != 1)
                {
                    GetOtherList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex);
                }
                else
                {
                    GetMemberList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex);
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }

        private static void GetMemberList(ISheet templateSheet, ISheet resultSheet, ref int rowId, int colId, ref int id, DataRowCollection dtr, ref List<object> values, int lastColumnIndex)
        {
            foreach (DataRow items in dtr)
            {
                if (id % 2 == 0)
                {
                    resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                }
                else
                {
                    resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                }

                values = new List<object>   
                    {
                        id,
                        items["username"].ToString(),
                        Convert.ToBoolean(items["closed"]) ? "Closed" : (Convert.ToBoolean(items["suspended"]) ? "Suspended" : (Convert.ToBoolean(items["enabled"]) ? "Open" : "Disabled")),
                        Convert.ToBoolean(items["DoubleCommLosing"]) ? "ON" : "OFF",
                        items["nickname"].ToString(),
                        items["firstname"].ToString(),
                        items["lastname"].ToString(),
                        items["ugroup"].ToString(),
                        Convert.ToDecimal(items["discount"]),
                        Convert.ToDecimal(items["discount1x2"]),
                        Convert.ToDecimal(items["discountcs"]),
                        Convert.ToDecimal(items["discountnumber"]),
                        Convert.ToDecimal(items["playerdiscount"]),
                        Convert.ToDecimal(items["playerdiscount1x2"]),
                        Convert.ToDecimal(items["playerdiscountcs"]),
                        Convert.ToDecimal(items["playerdiscountnumber"]),
                        String.Format("{0:yyyy-MM-dd HH:mm:ss}", items["createdate"]),
                        items["lastloginip"].ToString()
                    };

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }

        private static void GetOtherList(ISheet templateSheet, ISheet resultSheet, ref int rowId, int colId, ref int id, DataRowCollection dtr, ref List<object> values, int lastColumnIndex)
        {
            foreach (DataRow items in dtr)
            {
                if (id % 2 == 0)
                {
                    resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                }
                else
                {
                    resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                }

                values = new List<object>   
                    {
                        id,
                        items["username"].ToString(),
                        Convert.ToBoolean(items["closed"]) ? "Closed" : (Convert.ToBoolean(items["suspended"]) ? "Suspended" : (Convert.ToBoolean(items["enabled"]) ? "Open" : "Disabled")),
                        Convert.ToBoolean(items["DoubleCommLosing"]) ? "ALLOWED" : "DISALLOWED",
                        items["firstname"].ToString(),
                        items["lastname"].ToString(),
                        Convert.ToDecimal(items["groupa"]),
                        Convert.ToDecimal(items["groupb"]),
                        Convert.ToDecimal(items["groupc"]),
                        Convert.ToDecimal(items["groupd"]),
                        Convert.ToDecimal(items["discount1x2"]),
                        Convert.ToDecimal(items["discountcs"]),
                        Convert.ToDecimal(items["discountnumber"]),
                        String.Format("{0:yyyy-MM-dd HH:mm:ss}", items["createdate"]),
                        items["lastloginip"].ToString()
                    };

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }
    }
}
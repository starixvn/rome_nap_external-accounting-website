// --------------------------------------------------------------------------
// <copyright file="CustomerListPage.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

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
            string mainTitle = this.builder.Status + (this.builder.RoleId == 1 ? " Member" : (this.builder.RoleId == 2 ? " Agent" : (this.builder.RoleId == 3 ? " Master" : " Super"))) + " List";

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
                if (this.builder.RoleId != 1)
                {
                    GetOtherList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex);
                }
                else
                {
                    totalColumn = 18;
                    lastColumnIndex = colId + totalColumn - 1;
                    GetMemberList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex);
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }

        private static void GetMemberList(ISheet templateSheet, ISheet resultSheet, ref int rowId, int colId, ref int id, DataRowCollection dtr, ref List<object> values, int lastColumnIndex)
        {
            string custStatus = "Disabled";
            foreach (DataRow items in dtr)
            {
                custStatus = Convert.ToBoolean(items["closed"]) ? "Closed" : (Convert.ToBoolean(items["suspended"]) ? "Suspended" : (Convert.ToBoolean(items["enabled"]) ? "Open" : "Disabled"));
                if (custStatus.ToLower() == "open")
                {
                    resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "suspended")
                {
                    resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "closed")
                {
                    resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "disabled")
                {
                    resultSheet.CopyRow(templateSheet, 7, rowId, colId, lastColumnIndex);
                }

                values = new List<object>   
                    {
                        id,
                        items["username"].ToString(),
                        custStatus,
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
                        string.Format("{0:MM/dd/yyyy HH:mm:ss tt}", items["createdate"]),
                        items["lastloginip"].ToString()
                    };

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }

        private static void GetOtherList(ISheet templateSheet, ISheet resultSheet, ref int rowId, int colId, ref int id, DataRowCollection dtr, ref List<object> values, int lastColumnIndex)
        {
            string custStatus = "Disabled";
            foreach (DataRow items in dtr)
            {
                custStatus = Convert.ToBoolean(items["closed"]) ? "Closed" : (Convert.ToBoolean(items["suspended"]) ? "Suspended" : (Convert.ToBoolean(items["enabled"]) ? "Open" : "Disabled"));
                if (custStatus.ToLower() == "open")
                {
                    resultSheet.CopyRow(templateSheet, 1, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "suspended")
                {
                    resultSheet.CopyRow(templateSheet, 3, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "closed")
                {
                    resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
                }
                else if (custStatus.ToLower() == "disabled")
                {
                    resultSheet.CopyRow(templateSheet, 7, rowId, colId, lastColumnIndex);
                }

                values = new List<object>   
                    {
                        id,
                        items["username"].ToString(),
                        custStatus,
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
                        string.Format("{0:MM/dd/yyyy HH:mm:ss tt}", items["createdate"]),
                        items["lastloginip"].ToString()
                    };

                resultSheet.GetRow(rowId).WriteCells(colId, values);

                id++;
                rowId++;
            }
        }
    }
}
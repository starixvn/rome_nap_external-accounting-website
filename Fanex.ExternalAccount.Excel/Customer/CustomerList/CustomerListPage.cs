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

    /// <summary>
    /// Customer list.
    /// </summary>
    internal class CustomerListPage : TemplateReport, IReportBuilder
    {
        private CustomerListPageBuilder _builder;

        public CustomerListPage(string filePath, CustomerListPageBuilder builder)
            : base(filePath)
        {
            this._builder = builder;
        }

        /// <summary>
        /// Build workbook.
        /// </summary>
        /// <returns>HSSFWorkbook object.</returns>
        public HSSFWorkbook Build()
        {
            var data = this._builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("CustomerList");

            if (this._builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            string mainTitle = this._builder.Status + (this._builder.RoleId == 1 ? " Member" : 
                (this._builder.RoleId == 2 ? " Agent" : (this._builder.RoleId == 3 ? " Master" : " Super"))) + " List";

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
                if (this._builder.RoleId != 1)
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

        /// <summary>
        /// Gets member list.
        /// </summary>
        /// <param name="templateSheet">Template sheet.</param>
        /// <param name="resultSheet">Result sheet.</param>
        /// <param name="rowId">Row id of workbook.</param>
        /// <param name="colId">Column id.</param>
        /// <param name="id">Customer id.</param>
        /// <param name="dtr">Data rows.</param>
        /// <param name="values">Object values.</param>
        /// <param name="lastColumnIndex">Last column index.</param>
        private static void GetMemberList(
            ISheet templateSheet, 
            ISheet resultSheet, 
            ref int rowId, 
            int colId, 
            ref int id, 
            DataRowCollection dtr, 
            ref List<object> values, 
            int lastColumnIndex)
        {
            string custStatus = "Disabled";
            foreach (DataRow items in dtr)
            {
                custStatus = SetBackgroundRow(templateSheet, resultSheet, rowId, colId, lastColumnIndex, custStatus, items);

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

        /// <summary>
        /// Gets other list.
        /// </summary>
        /// <param name="templateSheet">Template sheet.</param>
        /// <param name="resultSheet">Result sheet.</param>
        /// <param name="rowId">Row id of workbook.</param>
        /// <param name="colId">Column id.</param>
        /// <param name="id">Customer id.</param>
        /// <param name="dtr">Data rows.</param>
        /// <param name="values">Object values.</param>
        /// <param name="lastColumnIndex">Last column index.</param>
        private static void GetOtherList(
            ISheet templateSheet, 
            ISheet resultSheet, 
            ref int rowId,
            int colId, 
            ref int id, 
            DataRowCollection dtr,
            ref List<object> values, 
            int lastColumnIndex)
        {
            string custStatus = "Disabled";
            foreach (DataRow items in dtr)
            {
                custStatus = SetBackgroundRow(templateSheet, resultSheet, rowId, colId, lastColumnIndex, custStatus, items);

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

        /// <summary>
        /// Sets background row.
        /// </summary>
        /// <param name="templateSheet">Template sheet.</param>
        /// <param name="resultSheet">Result sheet.</param>
        /// <param name="rowId">Row id of workbook.</param>
        /// <param name="colId">Column id.</param>
        /// <param name="lastColumnIndex">Last column index.</param>
        /// <param name="custStatus">Customer status.</param>
        /// <param name="items">Data rows.</param>
        /// <returns>String object.</returns>
        private static string SetBackgroundRow(
            ISheet templateSheet, 
            ISheet resultSheet, 
            int rowId, 
            int colId, 
            int lastColumnIndex, 
            string custStatus, 
            DataRow items)
        {
            custStatus = !Convert.ToBoolean(items["enabled"]) ? "Disabled" : Convert.ToBoolean(items["closed"]) ? 
                "Closed" : Convert.ToBoolean(items["suspended"]) ? "Suspended" : "Open";
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

            return custStatus;
        }
    }
}
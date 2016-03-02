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
    using System.Diagnostics.CodeAnalysis;
    using Fanex.ExternalAccount.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    /// <summary>
    /// Customer list.
    /// </summary>
    internal class CustomerListPage : TemplateReport, IReportBuilder
    {
        private static CustomerListPageBuilder _builder;

        public CustomerListPage(string filePath, CustomerListPageBuilder builder)
            : base(filePath)
        {
            _builder = builder;
        }

        /// <summary>
        /// Build workbook.
        /// </summary>
        /// <returns>HSSFWorkbook object.</returns>
        public HSSFWorkbook Build()
        {
            var data = _builder.Data;
            if (data == null)
            {
                return null;
            }

            ISheet templateSheet = HssfWorkbook.GetSheet("Template");
            ISheet resultSheet = HssfWorkbook.GetSheet("CustomerList");

            if (_builder.IsBold)
            {
                this.BoldAllFonts();
            }

            int rowId = 0;
            int colId = 0;

            string mainTitle = _builder.Status + (_builder.RoleId == 1 ? " Member" : 
                (_builder.RoleId == 2 ? " Agent" : (_builder.RoleId == 3 ? " Master" : " Super"))) + " List";

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 3;
            int id = 1;

            int count = data.Rows.Count;
            DataRowCollection dtr = data.Rows;

            if (count > 0)
            {
                var values = new List<object> { };
                int totalColumn = _builder.IsMyanmarOdds || _builder.RoleId == 4 ? 16 : 15;
                int lastColumnIndex = colId + totalColumn - 1;
                if (_builder.RoleId != 1)
                {
                    GetOtherList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex, _builder.IsMyanmarOdds);
                }
                else
                {
                    totalColumn = 20;
                    lastColumnIndex = colId + totalColumn - 1;
                    GetMemberList(templateSheet, resultSheet, ref rowId, colId, ref id, dtr, ref values, lastColumnIndex, _builder.IsMyanmarOdds);
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
            int lastColumnIndex,
            bool isMyanmarOdds = false)
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
                        string.Format("{0:MM/dd/yyyy HH:mm:ss tt}", items["createdate"]),
                        items["lastloginip"].ToString()
                    };

                if (isMyanmarOdds)
                {
                    values.Insert(12, Utils.ToBoolean(items["ismyanmarodds"]) ? Utils.ToDouble(items["discountmyanmarodds"]).ToString() : "N/A");
                    values.Insert(17, Utils.ToBoolean(items["ismyanmarodds"]) ? Utils.ToDouble(items["playerdiscountmyanmarodds"]).ToString() : "N/A");
                }

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
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static void GetOtherList(
            ISheet templateSheet, 
            ISheet resultSheet, 
            ref int rowId,
            int colId, 
            ref int id, 
            DataRowCollection dtr,
            ref List<object> values, 
            int lastColumnIndex,
            bool isMyanmarOdds = false)
        {
            string custStatus = "Disabled";
            foreach (DataRow items in dtr)
            {
                custStatus = SetBackgroundRow(templateSheet, resultSheet, rowId, colId, lastColumnIndex, custStatus, items);

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
                            string.Format("{0:MM/dd/yyyy HH:mm:ss tt}", items["createdate"]),
                            items["lastloginip"].ToString()
                        };

                        values.Insert(10, Utils.ToDouble(items["discountmyanmarodds"]).ToString());
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
                            string.Format("{0:MM/dd/yyyy HH:mm:ss tt}", items["createdate"]),
                            items["lastloginip"].ToString()
                        };

                        if (_builder.RoleId == 4 || Utils.ToBoolean(items["ismyanmarodds"]))
                        {
                            values.Insert(13, Utils.ToBoolean(items["ismyanmarodds"]) ? Utils.ToDouble(items["discountmyanmarodds"]).ToString() : "N/A");

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
            custStatus = !Utils.ToBoolean(items["enabled"]) ? "Disabled" : Utils.ToBoolean(items["closed"]) ? 
                "Closed" : Utils.ToBoolean(items["suspended"]) ? "Suspended" : "Open";
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
                resultSheet.CopyRow(templateSheet, 5, rowId, colId, lastColumnIndex);
            }
            else if (custStatus.Equals("disabled", StringComparison.OrdinalIgnoreCase))
            {
                resultSheet.CopyRow(templateSheet, 7, rowId, colId, lastColumnIndex);
            }

            return custStatus;
        }
    }
}
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
            string mainTitle = " Customer List";

            resultSheet.GetCell(rowId, colId).SetCellValue(mainTitle);

            // content
            rowId = 2;
            int id = 1;

            int count = data.Rows.Count;
            DataRowCollection dtr = data.Rows;

            if (count > 0)
            {
                var values = new List<object> { };
                int totalColumn = 4;
                int lastColumnIndex = colId + totalColumn - 1;

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
                        items["groupa"].ToString(),
                        items["groupb"].ToString(),
                        items["groupc"].ToString(),
                        items["groupd"].ToString(),
                        items["discount1x2"].ToString(),
                        items["discountcs"].ToString(),
                        items["discountnumber"].ToString(),
                        Convert.ToDateTime(items["createdate"]),
                        items["ip"].ToString()
                    };

                    resultSheet.GetRow(rowId).WriteCells(colId, values);

                    id++;
                    rowId++;
                }
            }

            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(templateSheet));
            return this.HssfWorkbook;
        }

        //private bool GetBool(string status)
        //{
            
        //}
    }
}
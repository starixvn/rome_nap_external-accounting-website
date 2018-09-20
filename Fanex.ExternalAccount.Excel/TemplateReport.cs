namespace Fanex.ExternalAccount.Excel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using Utility;

    public abstract class TemplateReport : IDisposable
    {
        private readonly string _products;
        protected const int OddRowIndex = 1;
        protected const int EvenRowIndex = 3;
        protected const int FirstRowIndex = 0;
        protected const int FirstColumnIndex = 0;
        protected const int FirstColumnTotalIndex = 5;

        protected TemplateReport(string filePath, string products = null)
        {
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            HssfWorkbook = new HSSFWorkbook(file);
            _products = products;
        }

        public ISheet TemplateSheet => HssfWorkbook.GetSheet("Template");

        public HSSFWorkbook Workbook => HssfWorkbook;

        public string Products => string.IsNullOrWhiteSpace(_products) ? string.Empty : _products;

        protected HSSFWorkbook HssfWorkbook { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemplateReport()
        {
            Dispose(false);
        }

        public void BoldAllFonts()
        {
            for (short i = 0; i < HssfWorkbook.NumberOfFonts; i++)
            {
                var font = HssfWorkbook.GetFontAt(i);
                font.Boldweight = (short)FontBoldWeight.Bold;
            }
        }

        public HSSFWorkbook Build(string sheetName, bool isBold)
        {
            var resultSheet = HssfWorkbook.GetSheet(sheetName);

            if (isBold)
            {
                BoldAllFonts();
            }

            BuildMainTitle(resultSheet);
            WriteBodyReport(resultSheet);
            HssfWorkbook.RemoveSheetAt(HssfWorkbook.GetSheetIndex(TemplateSheet));

            return HssfWorkbook;
        }

        public virtual void BuildMainTitle(ISheet resultSheet) { }

        public virtual void WriteBodyReport(ISheet resultSheet) { }

        public int WriteBodyReport(ISheet resultSheet, IEnumerable<List<object>> data, int lastColumnIndex)
        {
            var index = 1;
            var toRowId = 3;

            foreach (var item in data)
            {
                var fromRowId = index % 2 == 0 ? OddRowIndex : EvenRowIndex;
                resultSheet.CopyRow(TemplateSheet, fromRowId, toRowId, FirstColumnIndex, lastColumnIndex);
                resultSheet.GetRow(toRowId).WriteCells(FirstColumnIndex, item);
                index++;
                toRowId++;
            }

            return toRowId;
        }

        public void WriteTotal(ISheet resultSheet, int toRowIndex, List<object> data, int lastColumnIndex)
        {
            resultSheet.CopyRow(TemplateSheet, FirstColumnTotalIndex, toRowIndex, FirstColumnIndex, lastColumnIndex);
            resultSheet.GetRow(toRowIndex).WriteCells(FirstColumnIndex, data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && HssfWorkbook != null)
            {
                HssfWorkbook = null;
            }
        }
    }
}
namespace Fanex.ExternalAccount.Excel
{
    using System;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && HssfWorkbook != null)
            {
                HssfWorkbook = null;
            }
        }
    }
}
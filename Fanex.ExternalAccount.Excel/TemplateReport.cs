namespace Fanex.ExternalAccount.Excel
{
    using System;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Template report object.
    /// </summary>
    public abstract class TemplateReport : IDisposable
    {
        private HSSFWorkbook _hssfWorkbook;
        private string _products;

        public TemplateReport(string filePath, string products = null)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this._hssfWorkbook = new HSSFWorkbook(file);
            this._products = products;
        }

        ~TemplateReport()
        {
            // Finalizer calls Dispose(false)
            this.Dispose(false);
        }

        public HSSFWorkbook Workbook
        {
            get
            {
                return this._hssfWorkbook;
            }
        }

        public string Products
        {
            get
            {
                return string.IsNullOrWhiteSpace(this._products) ? string.Empty : this._products;
            }
        }

        protected HSSFWorkbook HssfWorkbook
        {
            get { return this._hssfWorkbook; }
            set { this._hssfWorkbook = value; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void BoldAllFonts()
        {
            for (short i = 0; i < this._hssfWorkbook.NumberOfFonts; i++)
            {
                var font = this._hssfWorkbook.GetFontAt(i);
                font.Boldweight = (short)FontBoldWeight.Bold;
            }
        }

        /// <summary>
        /// Dispose an object.
        /// </summary>
        /// <param name="disposing">Identifies to dispose or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this._hssfWorkbook != null)
            {
                this._hssfWorkbook = null;
            }
        }
    }
}
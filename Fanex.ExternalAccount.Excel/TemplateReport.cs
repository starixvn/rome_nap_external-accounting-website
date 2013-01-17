/*
 * Created 20120216@Leo
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Excel
{
    using System;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public abstract class TemplateReport : IDisposable
    {
        private HSSFWorkbook hssfWorkbook;

        public TemplateReport(string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this.hssfWorkbook = new HSSFWorkbook(file);
        }

        public HSSFWorkbook Workbook
        {
            get
            {
                return this.hssfWorkbook;
            }
        }

        protected HSSFWorkbook HssfWorkbook
        {
            get { return this.hssfWorkbook; }
            set { this.hssfWorkbook = value; }
        }

        public void Dispose()
        {
            if (this.hssfWorkbook != null)
            {
                this.hssfWorkbook.Dispose();
            }
        }

        public void BoldAllFonts()
        {
            for (short i = 0; i < this.hssfWorkbook.NumberOfFonts; i++)
            {
                var font = this.hssfWorkbook.GetFontAt(i);
                font.Boldweight = (short)FontBoldWeight.BOLD;
            }
        }
    }
}
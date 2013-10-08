// --------------------------------------------------------------------------
// <copyright file="TemplateReport.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Excel
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Template report object.
    /// </summary>
    public abstract class TemplateReport : IDisposable
    {
        private IntPtr _nativeResource = Marshal.AllocHGlobal(100);
        private HSSFWorkbook _hssfWorkbook;

        public TemplateReport(string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this._hssfWorkbook = new HSSFWorkbook(file);
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
                font.Boldweight = (short)FontBoldWeight.BOLD;
            }
        }

        /// <summary>
        /// Dispose an object.
        /// </summary>
        /// <param name="disposing">Identifies to dispose or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                if (this._hssfWorkbook != null)
                {
                    this._hssfWorkbook.Dispose();
                    this._hssfWorkbook = null;
                }
            }

            // free native resources if there are any.
            if (_nativeResource != IntPtr.Zero) 
            {
                Marshal.FreeHGlobal(_nativeResource);
                _nativeResource = IntPtr.Zero;
            }
        }
    }
}
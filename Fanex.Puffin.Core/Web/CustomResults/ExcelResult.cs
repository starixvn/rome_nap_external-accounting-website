// --------------------------------------------------------------------------
// <copyright file="ExcelResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.12">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ExcelUtil.Spreadsheet;

namespace Fanex.Puffin.Core.Web.CustomResults
{
    /// <summary>
    ///  A custom result with content type application/vnd.ms-excel
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ExcelResult : SpreadSheetResult
    {
        public ExcelResult(ExcelFile file, string fileName)
            : base(file, fileName) 
        { 
        }

        protected override string ContentType
        {
            get { return "application/vnd.ms-excel"; }
        }

        protected override string Extension
        {
            get { return ".xls"; }
        }

        protected override Action<ExcelFile, Stream> SaveAction
        {
            get { return (f, s) => f.SaveXls(s); }
        }
    }
}

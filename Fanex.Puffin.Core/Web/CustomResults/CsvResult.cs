// --------------------------------------------------------------------------
// <copyright file="CsvResult.cs" company="Nexcel Solutions Vietnam">
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
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class CsvResult : SpreadSheetResult
    {
        public CsvResult(ExcelFile file, string fileName)
            : base(file, fileName)
        {
        }

        protected override string ContentType
        {
            get { return "application/csv"; }
        }

        protected override string Extension
        {
            get { return ".csv"; }
        }

        protected override Action<ExcelFile, Stream> SaveAction
        {
            get { return (f, s) => f.SaveCsv(s, CsvType.CommaDelimited); }
        }
    }
}

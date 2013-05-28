// --------------------------------------------------------------------------
// <copyright file="SpreadSheetResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.12">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Mvc;
using ExcelUtil.Spreadsheet;

namespace Fanex.Puffin.Core.Web.CustomResults
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public abstract class SpreadSheetResult : ActionResult
    {
        private readonly ExcelFile _file;
        private readonly string _fileName;

        protected SpreadSheetResult(ExcelFile file, string fileName)
        {
            this._file = file;
            this._fileName = fileName;
        }

        protected abstract string ContentType { get; }

        protected abstract string Extension { get; }

        protected abstract Action<ExcelFile, Stream> SaveAction { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = this.ContentType;
            context.HttpContext.Response.AddHeader("Content-disposition", "filename=" + this._fileName + this.Extension);

            this.SaveAction(this._file, context.HttpContext.Response.OutputStream);
        }
    }
}

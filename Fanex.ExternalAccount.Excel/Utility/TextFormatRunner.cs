// --------------------------------------------------------------------------
// <copyright file="TextFormatRunner.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Diagnostics;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Text format checker.
    /// </summary>
    public static class TextFormatRunner
    {
        public static IRichTextString Run(RtfTextRender renderer, ITextFormat value)
        {
            Debug.Assert(renderer != null, "renderer must be not null");
            Debug.Assert(value != null, "value must be not null");
            return null;
        }
    }
}
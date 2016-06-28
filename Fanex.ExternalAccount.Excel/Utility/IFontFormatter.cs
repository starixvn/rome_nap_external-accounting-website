// --------------------------------------------------------------------------
// <copyright file="IFontFormatter.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.UserModel;

    public interface IFontFormatter
    {
        void Format(IFont font);
    }

    /// <summary>
    /// Font formatter.
    /// </summary>
    public class RtfFontFormatter : IFontFormatter
    {
        private string _fontName;
        private short _fontHeightInPoints;

        public RtfFontFormatter(string fontName, short fontHeightInPoints)
        {
            this._fontName = fontName;
            this._fontHeightInPoints = fontHeightInPoints;
        }

        public RtfFontFormatter(IFont font)
        {
            this._fontName = font.FontName;
            this._fontHeightInPoints = font.FontHeightInPoints;
        }

        public void Format(IFont font)
        {
            font.FontName = this._fontName;
            font.FontHeightInPoints = this._fontHeightInPoints;
        }
    }    
}
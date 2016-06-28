// --------------------------------------------------------------------------
// <copyright file="RtfTextRender.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Text renderer.
    /// </summary>
    public class RtfTextRender
    {
        private IFont _font;
        private List<TextBlock> _blocks = new List<TextBlock>();

        public RtfTextRender()
        {
        }

        public RtfTextRender(IFont font)
            : base()
        {
            this._font = font;
        }

        public void Clear()
        {
            this._blocks.Clear();
        }

        /// <summary>
        /// Render text string.
        /// </summary>
        /// <returns>IRichTextString object.</returns>
        public IRichTextString Render()
        {
            string s = string.Empty;
            foreach (var block in this._blocks)
            {
                s += block.Text;
            }

            IRichTextString result = new HSSFRichTextString(s);
            int textIndex = 0;
            foreach (var block in this._blocks)
            {
                IFont font = this._font;
                if (font == null)
                {
                    font = block.Font;
                }
                else
                {
                    if (block.Font != null)
                    {
                        font = block.Font;
                    }
                }

                if (font != null)
                {
                    int startIndex = textIndex;
                    int endIndex = textIndex + block.Text.Length;
                    result.ApplyFont(textIndex, endIndex, font);
                }

                textIndex += block.Text.Length;
            }

            return result;
        }

        public void AddText(TextBlock block)
        {
            this._blocks.Add(block);
        }

        public void AddText(string text)
        {
            this._blocks.Add(new TextBlock(text));
        }

        public void AddText(string text, IFont font)
        {
            this._blocks.Add(new TextBlock(text, font));
        }
    }
}
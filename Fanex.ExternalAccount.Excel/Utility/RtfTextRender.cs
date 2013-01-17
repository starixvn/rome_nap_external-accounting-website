/*
 * Created 20120216@Leo
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class RtfTextRender
    {
        private IFont font;
        private List<TextBlock> blocks = new List<TextBlock>();

        public RtfTextRender()
        {
        }

        public RtfTextRender(IFont font)
            : base()
        {
            this.font = font;
        }

        public void Clear()
        {
            this.blocks.Clear();
        }

        public IRichTextString Render()
        {
            string s = string.Empty;
            foreach (var block in this.blocks)
            {
                s += block.Text;
            }

            IRichTextString result = new HSSFRichTextString(s);
            int textIndex = 0;
            foreach (var block in this.blocks)
            {
                IFont font = this.font;
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
            this.blocks.Add(block);
        }

        public void AddText(string text)
        {
            this.blocks.Add(new TextBlock(text));
        }

        public void AddText(string text, IFont font)
        {
            this.blocks.Add(new TextBlock(text, font));
        }
    }
}
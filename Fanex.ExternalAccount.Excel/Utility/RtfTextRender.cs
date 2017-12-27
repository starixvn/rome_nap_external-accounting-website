namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using System.Text;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class RtfTextRender
    {
        private readonly IFont _font;
        private readonly List<TextBlock> _blocks = new List<TextBlock>();

        public RtfTextRender()
        {
        }

        public RtfTextRender(IFont font)
        {
            this._font = font;
        }

        public void Clear()
        {
            this._blocks.Clear();
        }

        public IRichTextString Render()
        {
            var content = new StringBuilder();
            foreach (var block in _blocks)
            {
                content.Append(block.Text);
            }

            IRichTextString result = new HSSFRichTextString(content.ToString());
            int textIndex = 0;
            foreach (var block in this._blocks)
            {
                var font = _font;
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
                    var endIndex = textIndex + block.Text.Length;
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
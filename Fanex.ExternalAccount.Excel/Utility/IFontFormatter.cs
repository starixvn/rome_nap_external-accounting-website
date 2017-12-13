using NPOI.SS.UserModel;

namespace Fanex.ExternalAccount.Excel.Utility
{
    public interface IFontFormatter
    {
        void Format(IFont font);
    }

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
namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.UserModel;

    public interface IFontFormatter
    {
        void Format(IFont font);
    }

    public class RtfFontFormatter : IFontFormatter
    {
        private string fontName;
        private short fontHeightInPoints;

        public RtfFontFormatter(string fontName, short fontHeightInPoints)
        {
            this.fontName = fontName;
            this.fontHeightInPoints = fontHeightInPoints;
        }

        public RtfFontFormatter(IFont font)
        {
            this.fontName = font.FontName;
            this.fontHeightInPoints = font.FontHeightInPoints;
        }

        public void Format(IFont font)
        {
            font.FontName = this.fontName;
            font.FontHeightInPoints = this.fontHeightInPoints;
        }
    }
}
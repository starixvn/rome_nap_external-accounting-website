namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.UserModel;

    public class TextBlock
    {
        private string _text;
        private IFont _font;

        public TextBlock(string text)
        {
            this._text = text;
        }

        public TextBlock(string text, IFont font)
        {
            this._text = text;
            this._font = font;
        }

        public string Text
        {
            get { return this._text; }
        }

        public IFont Font
        {
            get { return this._font; }
        }
    }
}
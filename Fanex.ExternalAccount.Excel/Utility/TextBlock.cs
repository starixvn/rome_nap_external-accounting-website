namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.UserModel;

    public class TextBlock
    {
        private string text;
        private IFont font;

        public TextBlock(string text)
        {
            this.text = text;
        }

        public TextBlock(string text, IFont font)
        {
            this.text = text;
            this.font = font;
        }

        public string Text
        {
            get { return this.text; }
        }

        public IFont Font
        {
            get { return this.font; }
        }
    }
}
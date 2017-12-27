namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.UserModel;

    public class TextBlock
    {
        public TextBlock(string text)
        {
            Text = text;
        }

        public TextBlock(string text, IFont font)
        {
            Text = text;
            Font = font;
        }

        public string Text { get; }

        public IFont Font { get; }
    }
}
namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Diagnostics;
    using NPOI.SS.UserModel;

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
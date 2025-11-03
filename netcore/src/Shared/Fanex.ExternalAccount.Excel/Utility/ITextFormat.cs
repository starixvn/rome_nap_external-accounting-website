namespace Fanex.ExternalAccount.Excel.Utility
{
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    public interface ITextFormat
    {
        Dictionary<string, IFont> Fonts { get; }

        string Text { get; }
    }
}
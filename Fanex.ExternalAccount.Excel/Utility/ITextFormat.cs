using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Fanex.ExternalAccount.Excel.Utility
{
    public interface ITextFormat
    {
        Dictionary<string, IFont> Fonts { get; }

        string Text { get; }
    }
}
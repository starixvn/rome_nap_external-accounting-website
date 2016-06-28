// --------------------------------------------------------------------------
// <copyright file="ITextFormat.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

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
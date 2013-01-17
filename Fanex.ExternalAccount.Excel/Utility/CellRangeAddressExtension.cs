// -----------------------------------------------------------------------
// <copyright file="CellRangeAddressExtension.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Utility
{
    using NPOI.SS.Util;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CellRangeAddressExtension
    {
        public static CellRangeAddress Clone(this CellRangeAddress range)
        {
            CellRangeAddress result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);
            return result;
        }
    }
}
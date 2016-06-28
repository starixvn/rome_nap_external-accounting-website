// --------------------------------------------------------------------------
// <copyright file="TemplateSettings.cs" company="">
// Copyright (c) . All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Web;

    /// <summary>
    /// Template setting of report.
    /// </summary>
    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "ExcelTemplates\\Reports";
        }

        /// <summary>
        /// Gets path of report.
        /// </summary>
        /// <param name="roleId">Role id of customer.</param>
        /// <returns>URL of report.</returns>
        public string WinLossDetailPath(int roleId)
        {
            if (roleId == 1)
            {
                return this._dir + "\\" + "WinLossDetailAg.xls";
            }
            else if (roleId == 2)
            {
                return this._dir + "\\" + "WinLossDetailMAg.xls";
            }
            else
            {
                return this._dir + "\\" + "WinLossDetail.xls";
            }
        }
    }
}
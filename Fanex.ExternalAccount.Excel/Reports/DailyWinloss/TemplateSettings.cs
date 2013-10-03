// --------------------------------------------------------------------------
// <copyright file="TemplateSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Web;

    /// <summary>
    /// Template setting of reports.
    /// </summary>
    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "\\ExcelTemplates\\Reports";
        }

        public string DailyWinLossPath()
        {
            return this._dir + "\\" + "DailyWinLoss.xls";
        }
    }
}
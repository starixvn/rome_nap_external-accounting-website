// --------------------------------------------------------------------------
// <copyright file="TemplateSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Web;

    /// <summary>
    /// Template URL for exporting excel.
    /// </summary>
    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "\\ExcelTemplates\\Customer";
        }

        public string CustomerListPath(int roleId)
        {
            if (roleId == 1)
            {
                return this._dir + "\\" + "MemberList.xls";
            }
            else
            {
                return this._dir + "\\" + "CustomerList.xls";
            }
        }
    }
}
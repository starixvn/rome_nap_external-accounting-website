namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Web;

    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "ExcelTemplates\\Reports";
        }

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
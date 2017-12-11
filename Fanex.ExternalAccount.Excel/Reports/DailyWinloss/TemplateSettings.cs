namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Web;

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
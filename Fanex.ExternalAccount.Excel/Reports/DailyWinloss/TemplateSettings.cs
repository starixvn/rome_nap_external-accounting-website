namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.Web;

    internal class TemplateSettings
    {
        private string dir;

        public TemplateSettings()
        {
            this.dir = HttpContext.Current.Server.MapPath("~") + "\\ExcelTemplates\\Reports";
        }

        public string DailyWinLossPath()
        {
            return this.dir + "\\" + "DailyWinLoss.xls";
        }
    }
}
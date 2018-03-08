namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.IO;
    using System.Web;

    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Reports");
        }

        public string DailyWinLossPath() => Path.Combine(_dir, "DailyWinLoss.xls");
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
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

        public string SuperWinLossDetailByPTPath() => Path.Combine(_dir, "SuperWinLossDetailByPT.xls");
        public string SuperWinLossDetailMCByPTPath() => Path.Combine(_dir, "SuperWinLossDetailMCByPT.xls");
    }
}
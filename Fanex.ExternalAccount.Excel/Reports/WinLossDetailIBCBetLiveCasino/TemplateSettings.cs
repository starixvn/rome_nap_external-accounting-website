namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
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

        public string WinLossDetailIBCBetLiveCasinoPath()
        {
            return Path.Combine(_dir, "WinLossDetailIBCBetLiveCasino.xls");
        }
    }
}
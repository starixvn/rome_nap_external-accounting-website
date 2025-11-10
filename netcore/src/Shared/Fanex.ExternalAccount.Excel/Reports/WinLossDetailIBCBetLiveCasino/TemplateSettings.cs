namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
{
    using System.IO;

    internal class TemplateSettings
    {
        private static string _basePath = "";
        private readonly string _dir;

        public static void SetBasePath(string basePath)
        {
            _basePath = basePath;
        }

        public TemplateSettings()
        {
            _dir = Path.Combine(_basePath, "ExcelTemplates", "Reports");
        }

        public string WinLossDetailIBCBetLiveCasinoPath()
        {
            return Path.Combine(_dir, "WinLossDetailIBCBetLiveCasino.xls");
        }
    }
}
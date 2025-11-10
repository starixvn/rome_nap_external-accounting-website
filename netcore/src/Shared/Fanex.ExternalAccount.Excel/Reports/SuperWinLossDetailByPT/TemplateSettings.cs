namespace Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT
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

        public string SuperWinLossDetailByPTPath() => Path.Combine(_dir, "SuperWinLossDetailByPT.xls");
        public string SuperWinLossDetailMCByPTPath() => Path.Combine(_dir, "SuperWinLossDetailMCByPT.xls");
    }
}
namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
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
            _dir = Path.Combine(_basePath, "ExcelTemplates", "Statement");
        }

        public string StatementPath() => Path.Combine(_dir, "Statement.xls");
    }
}
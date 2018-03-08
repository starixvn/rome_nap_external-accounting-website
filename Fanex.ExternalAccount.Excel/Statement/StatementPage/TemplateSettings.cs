namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.IO;
    using System.Web;

    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Statement");
        }

        public string StatementPath() => Path.Combine(_dir, "Statement.xls");
    }
}
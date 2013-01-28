namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.Web;

    internal class TemplateSettings
    {
        private string dir;

        public TemplateSettings()
        {
            this.dir = HttpContext.Current.Server.MapPath("~") + "\\ExcelTemplates\\Statement";
        }

        public string StatementPath()
        {
            return this.dir + "\\" + "Statement.xls";
        }
    }
}
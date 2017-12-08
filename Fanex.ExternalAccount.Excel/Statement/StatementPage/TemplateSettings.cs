namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.Web;

    /// <summary>
    /// Template setting of statement.
    /// </summary>
    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "\\ExcelTemplates\\Statement";
        }

        public string StatementPath()
        {
            return this._dir + "\\" + "Statement.xls";
        }
    }
}
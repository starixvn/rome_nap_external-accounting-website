namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Web;

    internal class TemplateSettings
    {
        private string dir;

        public TemplateSettings()
        {
            this.dir = HttpContext.Current.Server.MapPath("~") + "\\ExcelTemplates\\Customer";
        }

        public string CustomerListPath()
        {
            return this.dir + "\\" + "CustomerList.xls";
        }
    }
}
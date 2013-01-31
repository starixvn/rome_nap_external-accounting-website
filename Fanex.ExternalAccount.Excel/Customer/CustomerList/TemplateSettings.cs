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

        public string CustomerListPath(int roleId)
        {
            if (roleId == 1)
            {
                return this.dir + "\\" + "MemberList.xls";
            }
            else
            {
                return this.dir + "\\" + "CustomerList.xls";
            }
        }
    }
}
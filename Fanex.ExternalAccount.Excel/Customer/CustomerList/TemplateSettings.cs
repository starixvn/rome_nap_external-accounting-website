namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Web;

    internal class TemplateSettings
    {
        private string _dir;

        public TemplateSettings()
        {
            this._dir = HttpContext.Current.Server.MapPath("~/") + "\\ExcelTemplates\\Customer";
        }

        public string CustomerListPath(int roleId, bool isMyanmarOdds = false)
        {
            if (roleId == 1)
            {
                return this._dir + "\\" + (isMyanmarOdds ? "MemberListMO.xls" : "MemberList.xls");
            }
            else if (roleId == 4)
            {
                return this._dir + "\\SuperList.xls";
            }
            else
            {
                return this._dir + "\\" + (isMyanmarOdds ? "CustomerListMO.xls" : "CustomerList.xls");
            }
        }
    }
}
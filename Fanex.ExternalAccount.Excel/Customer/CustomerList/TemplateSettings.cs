namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.IO;
    using System.Web;

    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Customer");
        }

        public string CustomerListPath(int roleId, bool isMyanmarOdds = false)
        {
            switch (roleId)
            {
                case 1:
                    return Path.Combine(_dir, (isMyanmarOdds ? "MemberListMO.xls" : "MemberList.xls"));
                case 4:
                    return Path.Combine(_dir, "SuperList.xls");
                default:
                    return Path.Combine(_dir, (isMyanmarOdds ? "CustomerListMO.xls" : "CustomerList.xls"));
            }
        }
    }
}
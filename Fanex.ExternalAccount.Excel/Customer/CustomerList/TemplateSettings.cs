namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.IO;
    using System.Web;
    using Core.Shared;

    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Customer");
        }

        public string CustomerListPath(int roleId)
        {
            switch (roleId)
            {
                case Role.Member:
                    return Path.Combine(_dir, "MemberList.xls");
                case Role.Super:
                    return Path.Combine(_dir, "SuperList.xls");
                default:
                    return Path.Combine(_dir, "CustomerList.xls");
            }
        }
    }
}
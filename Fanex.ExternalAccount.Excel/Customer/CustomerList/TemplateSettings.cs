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

        public string CustomerListPath(int roleId, bool hasSubGroup = false, bool isMultipleCurrency = false)
        {
            var fileName = "";
            switch (roleId)
            {
                case Role.Member:
                    fileName = "MemberList";
                    break;
                case Role.Super:
                    fileName = "SuperList";
                    fileName += hasSubGroup ? "SG" : "";
                    break;
                default:
                    fileName = "CustomerList";
                    break;
            }
            
            fileName += isMultipleCurrency ? "MC.xls" : ".xls";

            return Path.Combine(_dir, fileName);
        }

        public string DirectMemberListPath(int roleId, bool isMultipleCurrency = false)
        {
            var fileName = "";
            switch (roleId)
            {
                case Role.Master:
                    fileName = "MasterDirectMemberList";
                    break;
                case Role.Super:
                    fileName = "SuperDirectMemberList";
                    break;
                default:
                    fileName = "MemberList";
                    break;
            }
            
            fileName += isMultipleCurrency ? "MC.xls" : ".xls";

            return Path.Combine(_dir, fileName);
        }
    }
}
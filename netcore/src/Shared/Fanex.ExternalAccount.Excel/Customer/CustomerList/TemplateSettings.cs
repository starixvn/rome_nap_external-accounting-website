namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.IO;
    using Core.Shared;

    internal class TemplateSettings
    {
        private static string _basePath = string.Empty;
        private readonly string _dir;

        public static void SetBasePath(string basePath)
        {
            _basePath = basePath;
        }

        public TemplateSettings()
        {
            _dir = Path.Combine(_basePath, "ExcelTemplates", "Customer");
        }

        public string CustomerListPath(int roleId, bool hasSubGroup = false, bool isMultipleCurrency = false)
        {
            var fileName = string.Empty;
            switch (roleId)
            {
                case Role.Member:
                    fileName = "MemberList";
                    break;
                case Role.Super:
                    fileName = "SuperList";
                    fileName += hasSubGroup ? "SG" : string.Empty;
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
            var fileName = string.Empty;
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

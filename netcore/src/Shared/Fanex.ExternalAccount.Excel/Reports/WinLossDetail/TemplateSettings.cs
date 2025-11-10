namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using Core.Shared;
    using System.IO;

    internal class TemplateSettings
    {
        private static string _basePath = "";
        private readonly string _dir;

        public static void SetBasePath(string basePath)
        {
            _basePath = basePath;
        }

        public TemplateSettings()
        {
            _dir = Path.Combine(_basePath, "ExcelTemplates", "Reports");
        }

        public string WinLossDetailPath(int uplineRoleId)
        {
            switch (uplineRoleId)
            {
                case Role.Agent:
                    return Path.Combine(_dir, "WinLossDetailAg.xls");
                case Role.Master:
                    return Path.Combine(_dir, "WinLossDetailMAg.xls");
                case Role.Super:
                    return Path.Combine(_dir, "WinLossDetail.xls");
                case Role.Group:
                    return Path.Combine(_dir, "WinLossDetailExtra.xls");
                default:
                    return "";
            }
        }

        public string WinLossByCurrencyDetailPath(int uplineRoleId)
        {
            switch (uplineRoleId)
            {
                case Role.Agent:
                    return Path.Combine(_dir, "WinLossMCDetailAg.xls");
                case Role.Master:
                    return Path.Combine(_dir, "WinLossMCDetailMAg.xls");
                case Role.Super:
                    return Path.Combine(_dir, "WinLossMCDetailSuper.xls");
                case Role.Group:
                    return Path.Combine(_dir, "WinLossMCDetail.xls");
                default:
                   return "";
            }
        }
        public static string GetLevelType(int roleId)
        {
            switch (roleId)
            {
                case Role.Master:
                    return "Master Agent";
                case Role.Agent:
                    return "Agent";
                default:
                    return "Direct member";
            }
        }

        public string CashFeePath()
        {
            return Path.Combine(_dir, "CashFee.xls");
        }
    }
}
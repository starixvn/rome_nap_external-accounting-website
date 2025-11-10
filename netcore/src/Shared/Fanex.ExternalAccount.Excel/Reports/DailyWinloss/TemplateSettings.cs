namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.IO;
    using Fanex.ExternalAccount.Core.Shared;
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

        public string DailyWinLossPath() => Path.Combine(_dir, "DailyWinLoss.xls");
        
        public string MasterDailyWinLossPath() => Path.Combine(_dir, "MasterDailyWinLoss.xls");
        
        public string AgentDailyWinLossPath() => Path.Combine(_dir, "AgentDailyWinLoss.xls");
        
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
    }
}
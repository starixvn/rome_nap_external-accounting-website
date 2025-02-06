namespace Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport
{
    using System.IO;
    using System.Web;
    using Fanex.ExternalAccount.Core.Shared;
    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Reports");
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
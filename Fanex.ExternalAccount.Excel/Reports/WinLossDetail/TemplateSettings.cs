namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.IO;
    using System.Web;
    using Core.Shared;

    internal class TemplateSettings
    {
        private readonly string _dir;

        public TemplateSettings()
        {
            _dir = HttpContext.Current.Server.MapPath("~/ExcelTemplates/Reports");
        }

        public string WinLossDetailPath(int roleId)
        {
            switch (roleId)
            {
                case Role.Member:
                    return Path.Combine(_dir, "WinLossDetailAg.xls");
                case Role.Agent:
                    return Path.Combine(_dir, "WinLossDetailMAg.xls");
                default:
                    return Path.Combine(_dir, "WinLossDetail.xls");
            }
        }
    }
}
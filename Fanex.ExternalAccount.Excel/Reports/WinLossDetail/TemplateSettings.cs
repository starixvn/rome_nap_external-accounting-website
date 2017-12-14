namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.IO;
    using System.Web;

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
                case 1:
                    return Path.Combine(_dir, "WinLossDetailAg.xls");
                case 2:
                    return Path.Combine(_dir, "WinLossDetailMAg.xls");
                default:
                    return Path.Combine(_dir, "WinLossDetail.xls");
            }
        }
    }
}
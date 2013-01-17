namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Web;

    internal class TemplateSettings
    {
        private string dir;

        public TemplateSettings()
        {
            this.dir = HttpContext.Current.Server.MapPath("~") + "ExcelTemplates\\Reports";
        }

        public string WinLossDetailPath(int roleId)
        {
            if (roleId == 1)
            {
                return this.dir + "\\" + "WinLossDetailAg.xls";
            }
            else if (roleId == 2)
            {
                return this.dir + "\\" + "WinLossDetailMAg.xls";
            }
            else
            {
                return this.dir + "\\" + "WinLossDetail.xls";
            }
        }
    }
}
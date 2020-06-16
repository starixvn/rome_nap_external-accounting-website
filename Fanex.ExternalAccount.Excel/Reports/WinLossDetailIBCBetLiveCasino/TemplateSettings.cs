namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport
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

        public string WinLossDetailIBCBetLiveCasinoPath(int roleId)
        {
            switch (roleId)
            {
                case Role.Member:
                    return Path.Combine(_dir, "WinLossDetailIBCBetLiveCasinoAg.xls");
                case Role.Agent:
                    return Path.Combine(_dir, "WinLossDetailIBCBetLiveCasinoMAg.xls");
                default:
                    return Path.Combine(_dir, "WinLossDetailIBCBetLiveCasino.xls");
            }
        }
    }
}

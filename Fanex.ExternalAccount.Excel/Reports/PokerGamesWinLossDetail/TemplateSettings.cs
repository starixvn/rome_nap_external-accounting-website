namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
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
                    return Path.Combine(_dir, "PokerGamesWinLossDetailAg.xls");
                case Role.Agent:
                    return Path.Combine(_dir, "PokerGamesWinLossDetailMAg.xls");
                default:
                    return Path.Combine(_dir, "PokerGamesWinLossDetail.xls");
            }
        }
    }
}
namespace Fanex.ExternalAccount.Excel.Reports.PokerGamesWinLossDetail
{
    using System.IO;

    public interface IPokerGamesWinLossDetailReportBuilder
    {
        bool IsBold { get; }
        int RoleId { get; }
        string FromDate { get; }
        string ToDate { get; }
        MemoryStream Build();
    }
}
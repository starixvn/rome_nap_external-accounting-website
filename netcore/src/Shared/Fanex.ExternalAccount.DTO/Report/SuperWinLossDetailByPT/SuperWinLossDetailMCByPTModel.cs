namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;
    using System.Linq;

    public class SuperWinLossDetailMCByPTModel
    {
        public SuperWinLossDetailMCByPTModel()
        {
        }

        public SuperWinLossDetailMCByPTModel(
        IEnumerable<SuperWinLossDetailMCByUsernameModel> winLossByUsernames,
        IEnumerable<SuperWinLossDetailMCByProductDTO> totalByCurrencies,
        string tooltipsForLiveCasino,
        string tooltipsForRNGCasino)
        {
            WinLossByUsernames = winLossByUsernames.OrderByDescending(winloss => winloss.Total.Turnover);
            TotalByCurrencies = totalByCurrencies;
            TooltipsForLiveCasino = tooltipsForLiveCasino;
            TooltipsForRNGCasino = tooltipsForRNGCasino;
        }

        public IEnumerable<SuperWinLossDetailMCByUsernameModel> WinLossByUsernames { get; }

        public IEnumerable<SuperWinLossDetailMCByProductDTO> TotalByCurrencies { get; set; }

        public string TooltipsForLiveCasino { get; }

        public string TooltipsForRNGCasino { get; }
    }
}
namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailMCByProductModel
    {
        public SuperWinLossDetailMCByProductModel()
        {
        }

        public SuperWinLossDetailMCByProductModel(
        IEnumerable<SuperWinLossDetailMCByProductDTO> winLossByProducts,
        IEnumerable<SuperWinLossDetailMCByProductDTO> totalByCurrencies)
        {
            WinLossByProducts = winLossByProducts;
            TotalByCurrencies = totalByCurrencies;
        }

        public IEnumerable<SuperWinLossDetailMCByProductDTO> WinLossByProducts { get; }

        public IEnumerable<SuperWinLossDetailMCByProductDTO> TotalByCurrencies { get; }
    }
}

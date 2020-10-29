namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailByProductModel
    {
        public SuperWinLossDetailByProductModel()
        {
        }

        public SuperWinLossDetailByProductModel(
        IEnumerable<SuperWinLossDetailByProductDTO> winLossByProducts,
        SuperWinLossDetailByProductDTO total)
        {
            WinLossByProducts = winLossByProducts;
            Total = total;
        }

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO Total { get; }
    }
}
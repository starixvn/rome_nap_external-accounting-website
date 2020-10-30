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
        SuperWinLossDetailByProductDTO total,
        int totalRecords)
        {
            WinLossByProducts = winLossByProducts;
            Total = total;
            TotalRecords = totalRecords;
        }

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO Total { get; }

        public int TotalRecords { get; }
    }
}
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
        SuperWinLossDetailByProductDTO grandTotal,
        int totalRecords)
        {
            WinLossByProducts = winLossByProducts;
            GrandTotal = grandTotal;
            TotalRecords = totalRecords;
        }

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO GrandTotal { get; }

        public int TotalRecords { get; }
    }
}
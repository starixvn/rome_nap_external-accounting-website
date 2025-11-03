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
            int totalRecords,
            SuperWinLossDetailByProductDTO dormantAccount)
        {
            WinLossByProducts = winLossByProducts;
            GrandTotal = grandTotal;
            TotalRecords = totalRecords;
            DormantAccount = dormantAccount;
        }

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO GrandTotal { get; }

        public int TotalRecords { get; }

        public SuperWinLossDetailByProductDTO DormantAccount { get; }
    }
}
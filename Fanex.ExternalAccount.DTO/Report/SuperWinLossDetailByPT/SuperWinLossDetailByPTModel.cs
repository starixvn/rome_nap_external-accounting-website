namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTModel
    {
        public SuperWinLossDetailByPTModel()
        {
        }

        public SuperWinLossDetailByPTModel(
            IEnumerable<SuperWinLossDetailByUsernameModel> winLossByUsernames,
            SuperWinLossDetailByProductDTO grandTotal,
            string tooltipsForLiveCasino,
            string tooltipsForRNGCasino,
            int totalRecords,
            int pageIndex,
            int pageSize)
        {
            WinLossByUsernames = winLossByUsernames;
            GrandTotal = grandTotal;
            TooltipsForLiveCasino = tooltipsForLiveCasino;
            TooltipsForRNGCasino = tooltipsForRNGCasino;
            TotalRecords = totalRecords;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public IEnumerable<SuperWinLossDetailByUsernameModel> WinLossByUsernames { get; }

        public SuperWinLossDetailByProductDTO GrandTotal { get; }

        public IEnumerable<SuperWinLossDetailMCByProductDTO> TotalByCurrencies { get; set; }

        public string TooltipsForLiveCasino { get; }

        public string TooltipsForRNGCasino { get; }

        public int TotalRecords { get; }

        public int PageIndex { get; }

        public int PageSize { get; }
    }
}
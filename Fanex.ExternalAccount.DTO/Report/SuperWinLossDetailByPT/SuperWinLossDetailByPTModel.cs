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
        SuperWinLossDetailByProductDTO total)
        {
            WinLossByUsernames = winLossByUsernames;
            Total = total;
        }

        public IEnumerable<SuperWinLossDetailByUsernameModel> WinLossByUsernames { get; }

        public SuperWinLossDetailByProductDTO Total { get; }
    }
}
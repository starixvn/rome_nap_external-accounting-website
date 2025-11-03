namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models
{
    using System.Collections.Generic;

    public class SuperWinLossDetailBySubGroupReport
    {
        public SuperWinLossDetailBySubGroupReport(
            IEnumerable<SuperWinLossDetailBySubGroupModel> superWinLossDetails,
            SuperWinLossDetailBySubGroupModel total,
            int totalItems)
        {
            SuperWinLossDetails = superWinLossDetails;
            Total = total;
            TotalItems = totalItems;
        }

        public IEnumerable<SuperWinLossDetailBySubGroupModel> SuperWinLossDetails { get; set; }

        public SuperWinLossDetailBySubGroupModel Total { get; }

        public int TotalItems { get; }
    }
}

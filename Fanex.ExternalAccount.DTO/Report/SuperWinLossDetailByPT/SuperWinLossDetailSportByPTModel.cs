namespace Fanex.ExternalAccount.DTO.Report
{
    using Fanex.ExternalAccount.DTO.Constants;
    using System.Collections.Generic;

    public class SuperWinLossDetailSportByPTModel
    {
        private static IDictionary<int, bool> productsHaveBetTypesGroupedBySport = new Dictionary<int, bool>
        {
            [ProductIdConstants.NG2] = true,
            [ProductIdConstants.VirtualSports] = true
        };

        public SuperWinLossDetailSportByPTModel(
            int sportId,
            string sportName,
            IEnumerable<WinLossByBetTypeModel> winLossByBetTypeData)
        {
            SportId = sportId;
            SportName = sportName;

            WinLossByBetTypeData = winLossByBetTypeData;
            AccumulateTotal();
        }

        public int SportId { get; }

        public string SportName { get; }

        public IEnumerable<WinLossByBetTypeModel> WinLossByBetTypeData { get; }

        public WinLossByBetTypeModel Total { get; private set; }

        public static bool IsShowBetTypeDirectly(int productId) => !productsHaveBetTypesGroupedBySport.ContainsKey(productId);

        public SuperWinLossDetailSportByPTModel AccumulateTotal()
        {
            Total = new WinLossByBetTypeModel();

            foreach (var winLossByBetTypeModel in WinLossByBetTypeData)
            {
                Total.Add(winLossByBetTypeModel);
            }

            return this;
        }
    }
}

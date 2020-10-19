namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTModel
    {
        public SuperWinLossDetailByPTModel()
        {
        }

        public SuperWinLossDetailByPTModel(
            int productId,
            string productName,
            IEnumerable<SuperWinLossDetailSportByPTModel> winLossBySportModels)
        {
            ProductId = productId;
            ProductName = productName;

            WinLossBySportModels = winLossBySportModels;
            AccumulateTotal();
        }

        public int ProductId { get; }

        public string ProductName { get; }

        public IEnumerable<SuperWinLossDetailSportByPTModel> WinLossBySportModels { get; }

        public WinLossByBetTypeModel Total { get; private set; }

        public SuperWinLossDetailByPTModel AccumulateTotal()
        {
            Total = new WinLossByBetTypeModel();

            foreach (var winLossBySportModel in WinLossBySportModels)
            {
                Total.Add(winLossBySportModel.Total);
            }

            return this;
        }
    }
}
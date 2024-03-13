namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;
    using System.Linq;

    public class SuperWinLossDetailByUsernameModel
    {
        public SuperWinLossDetailByUsernameModel()
        {
        }

        public SuperWinLossDetailByUsernameModel(IEnumerable<SuperWinLossDetailByProductDTO> winLossByProducts)
        {
            WinLossByProducts = winLossByProducts.OrderBy(winLossByProduct => winLossByProduct.PositionTaking);
            Total = CalculateTotal();
        }

        public SuperWinLossDetailByUsernameModel(SuperWinLossDetailByProductDTO total)
        {
            WinLossByProducts = Enumerable.Empty<SuperWinLossDetailByProductDTO>();
            Total = total;
        }

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO Total { get; }

        private SuperWinLossDetailByProductDTO CalculateTotal()
        {
            decimal turnover = 0;
            decimal netTurnover = 0;
            decimal grossComm = 0;
            decimal memberWinlost = 0;
            decimal memberComm = 0;
            decimal memberTotal = 0;
            decimal agWinlost = 0;
            decimal agComm = 0;
            decimal agTotal = 0;
            decimal magWinlost = 0;
            decimal magComm = 0;
            decimal magTotal = 0;
            decimal sagWinlost = 0;
            decimal sagComm = 0;
            decimal sagTotal = 0;
            decimal company = 0;

            foreach (var winLossByProduct in WinLossByProducts)
            {
                turnover += winLossByProduct.Turnover;
                netTurnover += winLossByProduct.NetTurnover;
                grossComm += winLossByProduct.GrossComm;

                memberWinlost += winLossByProduct.MemberWinlost;
                memberComm += winLossByProduct.MemberComm;
                memberTotal += winLossByProduct.MemberTotal;

                agWinlost += winLossByProduct.AgWinlost;
                agComm += winLossByProduct.AgComm;
                agTotal += winLossByProduct.AgTotal;

                magWinlost += winLossByProduct.MAgWinlost;
                magComm += winLossByProduct.MAgComm;
                magTotal += winLossByProduct.MAgTotal;

                sagWinlost += winLossByProduct.SAgWinlost;
                sagComm += winLossByProduct.SAgComm;
                sagTotal += winLossByProduct.SAgTotal;

                company += winLossByProduct.Company;
            }

            return new SuperWinLossDetailByProductDTO(
                WinLossByProducts.FirstOrDefault().CustId,
                WinLossByProducts.FirstOrDefault().Username,
                0,
                string.Empty,
                turnover,
                netTurnover,
                grossComm,
                memberWinlost,
                memberComm,
                memberTotal,
                agWinlost,
                agComm,
                agTotal,
                magWinlost,
                magComm,
                magTotal,
                sagWinlost,
                sagComm,
                sagTotal,
                company);
        }
    }
}
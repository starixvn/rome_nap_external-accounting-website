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
        }

        public int CustId => WinLossByProducts.FirstOrDefault().CustId;

        public string Username => WinLossByProducts.FirstOrDefault().Username;

        public IEnumerable<SuperWinLossDetailByProductDTO> WinLossByProducts { get; }

        public SuperWinLossDetailByProductDTO Total { get
            {
                var total = new SuperWinLossDetailByProductDTO();

                foreach (var winLossByProduct in WinLossByProducts)
                {
                    total.Turnover += winLossByProduct.Turnover;
                    total.NetTurnover += winLossByProduct.NetTurnover;
                    total.GrossComm += winLossByProduct.GrossComm;

                    total.MemberWinlost += winLossByProduct.MemberWinlost;
                    total.PlayerComm += winLossByProduct.PlayerComm;
                    total.MemberTotal += winLossByProduct.MemberTotal;

                    total.AgWinlost += winLossByProduct.AgWinlost;
                    total.AgComm += winLossByProduct.AgComm;
                    total.AgTotal += winLossByProduct.AgTotal;

                    total.MAgWinlost += winLossByProduct.MAgWinlost;
                    total.MAgComm += winLossByProduct.MAgComm;
                    total.MAgTotal += winLossByProduct.MAgTotal;

                    total.SAgWinlost += winLossByProduct.SAgWinlost;
                    total.SAgComm += winLossByProduct.SAgComm;
                    total.SAgTotal += winLossByProduct.SAgTotal;

                    total.Company += winLossByProduct.Company;
                }

                return total;
            }
        }
    }
}
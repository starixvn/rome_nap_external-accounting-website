namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailProductByUsername
    {
        public SuperWinLossDetailProductByUsername()
        {
        }

        public SuperWinLossDetailProductByUsername(
            int custId,
            string username,
            IEnumerable<SuperWinLossDetailByProductModel> winLossByProductModels)
        {
            CustId = custId;
            Username = username;

            WinLossByProductModels = winLossByProductModels;
        }

        public int CustId { get; }

        public string Username { get; }

        public IEnumerable<SuperWinLossDetailByProductModel> WinLossByProductModels { get; }

        public SuperWinLossDetailByProductModel Total { get
            {
                var total = new SuperWinLossDetailByProductModel();
                foreach (var winLossByProductModel in WinLossByProductModels)
                {
                    total.Turnover += winLossByProductModel.Turnover;
                    total.NetTurnover += winLossByProductModel.NetTurnover;
                    total.GrossComm += winLossByProductModel.GrossComm;
                    total.MemberWinlost += winLossByProductModel.MemberWinlost;
                    total.PlayerComm += winLossByProductModel.PlayerComm;
                    total.MemberTotal += winLossByProductModel.MemberTotal;

                    total.AgWinlost += winLossByProductModel.AgWinlost;
                    total.AgComm += winLossByProductModel.AgComm;
                    total.AgTotal += winLossByProductModel.AgTotal;
                    total.MAgWinlost += winLossByProductModel.MAgWinlost;
                    total.MAgComm += winLossByProductModel.MAgComm;
                    total.MAgTotal += winLossByProductModel.MAgTotal;

                    total.SAgWinlost += winLossByProductModel.SAgWinlost;
                    total.SAgComm += winLossByProductModel.SAgComm;
                    total.SAgTotal += winLossByProductModel.SAgTotal;
                    total.Company += winLossByProductModel.Company;
                }
                return total;
            }
        }
    }
}
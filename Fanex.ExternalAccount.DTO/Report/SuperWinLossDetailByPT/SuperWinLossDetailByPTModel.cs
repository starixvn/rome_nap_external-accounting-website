namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTModel
    {
        public SuperWinLossDetailByPTModel()
        {
        }

        public SuperWinLossDetailByPTModel(IEnumerable<SuperWinLossDetailProductByUsername> superWinLossDetailProductByUsernames)
        {
            SuperWinLossDetailProductByUsernames = superWinLossDetailProductByUsernames;
        }

        public IEnumerable<SuperWinLossDetailProductByUsername> SuperWinLossDetailProductByUsernames { get; }

        public SuperWinLossDetailByProductModel Total { get
            {
                var total = new SuperWinLossDetailByProductModel();

                foreach (var superWinLossDetailProductByUsername in SuperWinLossDetailProductByUsernames)
                {
                    var superWinLossDetailProductByUsernameTotal = superWinLossDetailProductByUsername.Total;
                    total.Turnover += superWinLossDetailProductByUsernameTotal.Turnover;
                    total.NetTurnover += superWinLossDetailProductByUsernameTotal.NetTurnover;
                    total.GrossComm += superWinLossDetailProductByUsernameTotal.GrossComm;
                    total.MemberWinlost += superWinLossDetailProductByUsernameTotal.MemberWinlost;
                    total.PlayerComm += superWinLossDetailProductByUsernameTotal.PlayerComm;
                    total.MemberTotal += superWinLossDetailProductByUsernameTotal.MemberTotal;

                    total.AgWinlost += superWinLossDetailProductByUsernameTotal.AgWinlost;
                    total.AgComm += superWinLossDetailProductByUsernameTotal.AgComm;
                    total.AgTotal += superWinLossDetailProductByUsernameTotal.AgTotal;
                    total.MAgWinlost += superWinLossDetailProductByUsernameTotal.MAgWinlost;
                    total.MAgComm += superWinLossDetailProductByUsernameTotal.MAgComm;
                    total.MAgTotal += superWinLossDetailProductByUsernameTotal.MAgTotal;

                    total.SAgWinlost += superWinLossDetailProductByUsernameTotal.SAgWinlost;
                    total.SAgComm += superWinLossDetailProductByUsernameTotal.SAgComm;
                    total.SAgTotal += superWinLossDetailProductByUsernameTotal.SAgTotal;
                    total.Company += superWinLossDetailProductByUsernameTotal.Company;
                }

                return total;
            }
        }
    }
}
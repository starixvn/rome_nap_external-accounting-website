namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class PokerGamesWinLossDetailAg
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public double Turnover { get; set; }

        public double NetTurnover { get; set; }

        public double TaxComm { get; set; }

        public double MemberWinLoss { get; set; }

        public double MemberInsuranceGemsWinLoss { get; set; }

        public double MemberComm { get; set; }

        public double MemberTotal { get; set; }

        public double AgInsuranceGemsWinLoss { get; set; }

        public double AgComm { get; set; }

        public double AgTotal { get; set; }

        public double Company { get; set; }

        public List<object> GetExcelColumns(string usernameColumn = null)
        {
            var values = new List<object>
            {
                Username,
                Turnover,
                NetTurnover,
                TaxComm,
                MemberWinLoss,
                MemberInsuranceGemsWinLoss,
                MemberComm,
                MemberTotal,
                AgInsuranceGemsWinLoss,
                AgComm,
                AgTotal
            };

            values.AddRange(GetExtraExcelColumns());
            values.Add(Company);
            return values;
        }

        public virtual List<object> GetExtraExcelColumns()
            => new List<object>();

        public virtual void Sum(PokerGamesWinLossDetailAg p)
        {
            Turnover += p.Turnover;
            NetTurnover += p.NetTurnover;
            TaxComm += p.TaxComm;
            MemberWinLoss += p.MemberWinLoss;
            MemberInsuranceGemsWinLoss += p.MemberInsuranceGemsWinLoss;
            MemberComm += p.MemberComm;
            MemberTotal += p.MemberTotal;
            AgInsuranceGemsWinLoss += p.AgInsuranceGemsWinLoss;
            AgComm += p.AgComm;
            AgTotal += p.AgTotal;
            Company += p.Company;
        }
    }
}
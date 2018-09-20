namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class PokerGamesWinLossDetail : PokerGamesWinLossDetailMAg
    {
        public double SAgInsuranceGemsWinLoss { get; set; }

        public double SAgComm { get; set; }

        public double SAgTotal { get; set; }

        public override List<object> GetExtraExcelColumns()
        {
            var values = base.GetExtraExcelColumns();

            values.Add(SAgInsuranceGemsWinLoss);
            values.Add(SAgComm);
            values.Add(SAgTotal);

            return values;
        }

        public override void Sum(PokerGamesWinLossDetailAg p)
        {
            base.Sum(p);

            SAgInsuranceGemsWinLoss += ((PokerGamesWinLossDetail)p).SAgInsuranceGemsWinLoss;
            SAgComm += ((PokerGamesWinLossDetail)p).SAgComm;
            SAgTotal += ((PokerGamesWinLossDetail)p).SAgTotal;
        }
    }
}
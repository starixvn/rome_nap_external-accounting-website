namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Collections.Generic;

    public class PokerGamesWinLossDetailMAg : PokerGamesWinLossDetailAg
    {
        public double MAgInsuranceGemsWinLoss { get; set; }

        public double MAgComm { get; set; }

        public double MAgTotal { get; set; }

        public override List<object> GetExtraExcelColumns()
        {
            var values = base.GetExtraExcelColumns();

            values.Add(MAgInsuranceGemsWinLoss);
            values.Add(MAgComm);
            values.Add(MAgTotal);

            return values;
        }

        public override void Sum(PokerGamesWinLossDetailAg p)
        {
            base.Sum(p);

            MAgInsuranceGemsWinLoss += ((PokerGamesWinLossDetailMAg)p).MAgInsuranceGemsWinLoss;
            MAgComm += ((PokerGamesWinLossDetailMAg)p).MAgComm;
            MAgTotal += ((PokerGamesWinLossDetailMAg)p).MAgTotal;
        }
    }
}
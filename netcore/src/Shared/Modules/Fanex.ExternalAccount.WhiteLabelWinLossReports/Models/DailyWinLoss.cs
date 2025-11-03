namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Models
{
    using System;

    public class DailyWinLoss
    {
        public DateTime? WinLostDate { get; set; }
        public string WhiteLabelName { get; set; }
        public string Super { get; set; }
        public string Master { get; set; }
        public string Agent { get; set; }
        public decimal MemberWinLoss { get; set; }
        public decimal Promotion { get; set; }

        public static DailyWinLoss operator +(DailyWinLoss model1, DailyWinLoss model2)
        {
            model1.MemberWinLoss += model2.MemberWinLoss;
            model1.Promotion += model2.Promotion;

            return model1;
        }
    }
}
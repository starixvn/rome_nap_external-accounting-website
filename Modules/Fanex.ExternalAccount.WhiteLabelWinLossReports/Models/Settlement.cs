namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Models
{
    public class Settlement
    {
        public string Month { get; set; }
        public string WhiteLabel { get; set; }
        public string Super { get; set; }
        public string Master { get; set; }
        public string Agent { get; set; }
        
        public decimal MemberWL { get; set; }
        public decimal MemberPromotion { get; set; }
        
        public decimal LossCarryforward { get; set; }
        public decimal WhiteLabelWinLoss { get; set; }
        public decimal WhiteLabelNetWinLoss { get; set; }
        public decimal WhiteLabelPromotionShare { get; set; }
        public decimal WhiteLabelFee { get; set; }
        public decimal WhiteLabelPromotionSettlement { get; set; }
        public decimal WhiteLabelTotal { get; set; }
        
        public decimal MasterEarning { get; set; }
        public decimal SuperEarning { get; set; }
        public decimal HeadlineEarning { get; set; }
        
        public decimal CompanyIncome { get; set; }
        public decimal CompanyPromotionShare { get; set; }
        public decimal CompanyTotal { get; set; }

        public static Settlement operator +(Settlement model1, Settlement model2)
        {
            model1.MemberWL += model2.MemberWL;
            model1.MemberPromotion += model2.MemberPromotion;
            
            model1.LossCarryforward += model2.LossCarryforward;
            model1.WhiteLabelWinLoss += model2.WhiteLabelWinLoss;
            model1.WhiteLabelNetWinLoss += model2.WhiteLabelNetWinLoss;
            model1.WhiteLabelPromotionShare += model2.WhiteLabelPromotionShare;
            model1.WhiteLabelFee += model2.WhiteLabelFee;
            model1.WhiteLabelPromotionSettlement += model2.WhiteLabelPromotionSettlement;
            model1.WhiteLabelTotal += model2.WhiteLabelTotal;
            
            model1.MasterEarning += model2.MasterEarning;
            model1.SuperEarning += model2.SuperEarning;
            model1.HeadlineEarning += model2.HeadlineEarning;
            
            model1.CompanyIncome += model2.CompanyIncome;
            model1.CompanyPromotionShare += model2.CompanyPromotionShare;
            model1.CompanyTotal += model2.CompanyTotal;

            return model1;
        }
    }
}
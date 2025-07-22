namespace Fanex.ExternalAccount.DTO.Report
{
    public class CashFeeItem
    {
        public int FeeType { get; set; }

        public int Type { get; set; }

        public string DisplayType { get; set; }

        public int Count { get; set; }

        public decimal Fee { get; set; }

        public decimal Agent { get; set; }

        public decimal Company { get; set; }

        public static CashFeeItem operator +(CashFeeItem model1, CashFeeItem model2)
        {
            model1.Count += model2.Count;
            model1.Fee += model2.Fee;
            model1.Agent += model2.Agent;
            model1.Company += model2.Company;

            return model1;
        }
    }
}

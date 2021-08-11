namespace Fanex.ExternalAccount.ViewModel.Customer.ParlayMaxPayout
{
    using Fanex.ExternalAccount.Core.Shared;
    using System.Collections.Generic;

    public class ParlayMaxPayoutViewModel
    {
        public int CustId { get; set; }

        public string CustIds { get; set; }

        public string CustName { get; set; }

        public string CustNames { get; set; }

        public int RoleId { get; set; }

        public bool IsMultiple => !string.IsNullOrWhiteSpace(CustIds);

        public List<ParlayMaxPayoutEntityModel> ListEntityModel { get; set; }
    }

    public class ParlayMaxPayoutEntityModel
    {
        public int SportType { get; set; }

        public string SportName => CacheRef.GetSportName(SportType.ToString());

        public int Bettype { get; set; }

        public string BettypeName => CacheRef.GetBettype(Bettype.ToString());

        public decimal SingleMatchParlay { get; set; }

        public decimal UplineSingleMatchParlay { get; set; }

        public bool IsAllowUnlimitedSingleMatchParlay { get; set; }

        public decimal Combo2 { get; set; }

        public decimal UplineCombo2 { get; set; }

        public bool IsAllowUnlimitedCombo2 { get; set; }

        public decimal Combo35 { get; set; }

        public decimal UplineCombo35 { get; set; }

        public bool IsAllowUnlimitedCombo35 { get; set; }

        public decimal ComboOver6 { get; set; }

        public decimal UplineComboOver6 { get; set; }

        public bool IsAllowUnlimitedComboOver6 { get; set; }
    }
}

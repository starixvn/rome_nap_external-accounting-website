namespace Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout
{
    using System.Collections.Generic;

    public class ParlayMaxPayoutUpdateMultipleRequest: ParlayMaxPayoutUpdateRequest
    {
        public string CustIds { get; set; }
    }

    public class ParlayMaxPayoutUpdateRequest
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }

        public List<ParlayMaxPayoutUpdateEntityModel> ParlayMaxPayoutList { get; set; }
    }

    public class ParlayMaxPayoutUpdateEntityModel
    {
        public int SportType { get; set; }

        public int Bettype { get; set; }

        public decimal? SingleMatchParlay { get; set; }

        public decimal? Combo2 { get; set; }

        public decimal? Combo35 { get; set; }

        public decimal? ComboOver6 { get; set; }
    }
}

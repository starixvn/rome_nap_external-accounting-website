namespace Fanex.Tarus.Customer.Criteria.ParlayMaxPayout
{
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;

    public class MaxPayoutUpdateCriteria : MaxPayoutCheckCriteria
    {
        public MaxPayoutUpdateCriteria()
        { }

        public MaxPayoutUpdateCriteria(ParlayMaxPayoutUpdateRequest request)
        {
            this.CustId = request.CustId;
            this.IP = request.IP;
            this.UserId = request.UserId;
            this.MaxPayoutLimit = request.ParlayMaxPayoutList?.Count > 0 ? base.ListMaxPayoutToXML(request.ParlayMaxPayoutList) : string.Empty;
        }

        public int UserId { get; set; }

        public string IP { get; set; }

        public override bool IsValid()
        {
            return CustId > 0 && UserId > 0 && !string.IsNullOrWhiteSpace(MaxPayoutLimit);
        }

        public override string GetSettingKey()
        {
            return "Acc_CustSetting_MaxPayoutUpd";
        }
    }
}

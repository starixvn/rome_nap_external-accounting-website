namespace Fanex.Tarus.Customer.Criteria.ParlayMaxPayout
{
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;
    using Fanex.NAP.Framework.Data;
    using System.Collections.Generic;
    using System.Text;

    public class MaxPayoutUpdateCriteria : CriteriaBase
    {
        public MaxPayoutUpdateCriteria()
        { }

        public MaxPayoutUpdateCriteria(ParlayMaxPayoutUpdateRequest request)
        {
            this.CustId = request.CustId;
            this.IP = request.IP;
            this.SystemId = request.SystemId;
            this.UserId = request.UserId;
            this.MaxPayoutLimit = this.ListMaxPayoutToXML(request.ParlayMaxPayoutList);
        }

        public int CustId { get; set; }

        public string MaxPayoutLimit { get; set; }

        public static int SubAccId => 0;

        public int UserId { get; set; }

        public string IP { get; set; }

        public byte SystemId { get; set; }

        public override bool IsValid()
        {
            return CustId > 0 && UserId > 0;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustSetting_MaxPayoutUpd";
        }

        private string ListMaxPayoutToXML(List<ParlayMaxPayoutUpdateEntityModel> listMaxpayout)
        {
            StringBuilder builder = new StringBuilder("<ROOT>");
            foreach (var maxpayout in listMaxpayout)
            {
                builder.Append("<MaxPayoutLimit ");
                builder.Append($"SportType=\"{maxpayout.SportType}\" ");
                builder.Append($"BetType=\"{maxpayout.Bettype}\" ");
                builder.Append($"SingleMatchParlay=\"{maxpayout.SingleMatchParlay}\" ");
                builder.Append($"Combo2=\"{maxpayout.Combo2}\" ");
                builder.Append($"Combo35=\"{maxpayout.Combo35}\" ");
                builder.Append($"ComboOver6=\"{maxpayout.ComboOver6}\" ");
                builder.Append("/>");
            }

            builder.Append("</ROOT>");
            return builder.ToString();
        }
    }
}

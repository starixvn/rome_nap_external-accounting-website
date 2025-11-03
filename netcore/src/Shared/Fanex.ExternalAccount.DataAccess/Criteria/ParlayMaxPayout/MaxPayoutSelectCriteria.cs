namespace Fanex.ExternalAccount.DataAccess.ParlayMaxPayout
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;

    public class MaxPayoutSelectCriteria : CriteriaBase
    {
        public MaxPayoutSelectCriteria() { }

        public MaxPayoutSelectCriteria(ParlayMaxPayoutSelectRequest request)
        {
            CustId = request.CustId;
        }

        public int CustId { get; set; }

        public int SettingStatus { get; set; }

        public override bool IsValid()
        {
            return CustId > 0 && SettingStatus >= 0 && SettingStatus <= 1;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustSetting_MaxPayoutSelect";
        }
    }
}

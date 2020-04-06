namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class CloneSettingExtraPTCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int ExistingCustId { get; set; }

        public int Creator { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid() =>
            CustId > 0
            && ExistingCustId > 0
            && Creator > 0;

        public override string GetSettingKey() => "EAS_CustomerInsMember_CloneSetting_ExtraPT";
    }
}

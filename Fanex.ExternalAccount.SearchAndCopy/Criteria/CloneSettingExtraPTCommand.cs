namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.Data.Repository;

    public class CloneSettingExtraPTCommand : INonQueryCommand
    {
        public int CustId { get; set; }

        public int ExistingCustId { get; set; }

        public int Creator { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public bool IsValid() =>
            CustId > 0
            && ExistingCustId > 0
            && Creator > 0;

        public string GetSettingKey() => "EAS_CustomerInsMember_CloneSetting_ExtraPT";
    }
}

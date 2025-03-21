namespace Fanex.ExternalAccount.DataAccess.Criteria.IPInformation
{
    using Fanex.Data.Repository;

    public class GetIPInformationCriteria : CriteriaBase
    {
        public string IP { get; set; }

        public override string GetSettingKey() => "GetIPInfo";

        public override bool IsValid() => true;
    }
}

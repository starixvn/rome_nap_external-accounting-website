namespace Fanex.ExternalAccount.DataAccess.Criteria.IPInformation
{
    using Fanex.NAP.Framework.Data;

    public class GetIPInformationCriteria : CriteriaBase
    {
        public string IP { get; set; }

        public override string GetSettingKey() => "GetIPInfo";
    }
}

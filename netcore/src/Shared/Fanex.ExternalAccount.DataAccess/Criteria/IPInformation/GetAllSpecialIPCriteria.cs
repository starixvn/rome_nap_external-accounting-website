namespace Fanex.ExternalAccount.DataAccess.Criteria.IPInformation
{
    using Fanex.Data.Repository;

    public class GetAllSpecialIPCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "GetIPVN";

        public override bool IsValid() => true;
    }
}

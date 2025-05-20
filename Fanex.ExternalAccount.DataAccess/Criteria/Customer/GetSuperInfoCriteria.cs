namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetSuperInfoCriteria : CriteriaBase
    {
        public GetSuperInfoCriteria(int superId)
        {
            CustId = superId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "EAS_CustomerSelSuperInfo";

        public override bool IsValid()
            => CustId > 0;
    }
}

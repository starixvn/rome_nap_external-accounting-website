namespace Fanex.ExternalAccount.DataAccess.Customer
{
    using Fanex.Data.Repository;

    public class GetCustDBConfigByCustCriteria : CriteriaBase
    {
        public GetCustDBConfigByCustCriteria() : this(null, null) { }

        public GetCustDBConfigByCustCriteria(int? custId) : this(custId, null) { }

        public GetCustDBConfigByCustCriteria(string userName) : this(null, userName) { }

        public GetCustDBConfigByCustCriteria(int? custId, string userName)
        {
            CustId = custId;
            UserName = userName;
        }

        public int? CustId { get; }

        public string UserName { get; }

        public override string GetSettingKey() => "Acc_ServerDBConfigSelByCust";

        public override bool IsValid() => CustId > 0 || !string.IsNullOrWhiteSpace(UserName);
    }
}

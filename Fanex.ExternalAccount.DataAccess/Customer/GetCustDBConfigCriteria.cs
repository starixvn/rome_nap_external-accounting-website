using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Customer
{
    public class GetCustDBConfigByCustCriteria : CriteriaBase
    {
        public GetCustDBConfigByCustCriteria(int? custId = null, string userName = null)
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

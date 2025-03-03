using Fanex.Data.Repository;
namespace Fanex.ExternalAccount.DataAccess.Customer
{
    using Fanex.Data.Repository;

    public class GetCustDBConfigBySiteCriteria : CriteriaBase
    {
        public GetCustDBConfigBySiteCriteria() : this(null) { }

        public GetCustDBConfigBySiteCriteria(string site)
        {
            Site = site;
        }

        public string Site { get; }

        public override string GetSettingKey() => "Acc_ServerDBConfigSel";

        public override bool IsValid() => true;
    }
}

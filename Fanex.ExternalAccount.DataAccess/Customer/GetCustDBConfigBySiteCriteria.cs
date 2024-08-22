using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Customer
{
    public class GetCustDBConfigBySiteCriteria : CriteriaBase
    {
        public GetCustDBConfigBySiteCriteria(string site = null)
        {
            Site = site;
        }

        public string Site { get; }

        public override string GetSettingKey() => "Acc_ServerDBConfigSel";

        public override bool IsValid() => true;
    }
}

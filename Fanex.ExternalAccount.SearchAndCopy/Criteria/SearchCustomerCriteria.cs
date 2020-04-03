namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class SearchCustomerCriteria : CriteriaBase
    {
        public SearchCustomerCriteria(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public override string GetSettingKey() => "Acc_CustomerUplineUserNameSel";

        public override bool IsValid() => true;
    }
}

namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.NAP.Framework.Data;

    public class CheckMarkAccountCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public string Usernames { get; set; }

        public override string GetSettingKey()
            => "EAS_AccountType_Check";
    }
}

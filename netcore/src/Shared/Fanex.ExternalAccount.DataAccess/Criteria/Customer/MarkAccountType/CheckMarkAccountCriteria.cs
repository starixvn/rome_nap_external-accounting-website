namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType
{
    using Fanex.Data.Repository;

    public class CheckMarkAccountCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public string Usernames { get; set; }

        public override string GetSettingKey()
            => "EAS_AccountType_Check";

        public override bool IsValid() => true;
    }
}

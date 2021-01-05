namespace Fanex.ExternalAccount.HeadlineBonus.Criteria
{
    using Fanex.NAP.Framework.Data;

    internal class AddNewBonusCriteria : CriteriaBase
    {
        public AddNewBonusCriteria(
            string groupName,
            string usernames,
            decimal amount,
            int userId,
            string ip)
        {
            GroupName = groupName;
            Usernames = usernames;
            Amount = amount;
            UserId = userId;
            LoginIP = ip;
        }

        public string GroupName { get; }

        public string Usernames { get; }

        public decimal Amount { get; }

        public int UserId { get; }

        public string LoginIP { get; }

        public override string GetSettingKey()
            => "EAS_Bonus_Ins";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName)
            && !string.IsNullOrWhiteSpace(Usernames)
            && Amount > 0;
    }
}

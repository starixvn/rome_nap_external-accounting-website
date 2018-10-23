namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.NAP.Framework.Data;

    public class CheckMemberCreationCriteria : CriteriaBase
    {
        public int AgentId { get; set; }

        public static readonly string Language = "E";

        public override string GetSettingKey()
            => "Age_MaxMemberLimitBySuperCheck";

        public override bool IsValid()
            => AgentId > 0;
    }
}

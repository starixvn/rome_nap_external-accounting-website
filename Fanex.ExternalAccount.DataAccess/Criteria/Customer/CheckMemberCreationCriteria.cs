namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.NAP.Framework.Data;

    public class CheckMemberCreationCriteria : CriteriaBase
    {
        public int AgentId { get; set; }

        public string Language
        {
            get
            {
                return "E";
            }
        }

        public override string GetSettingKey()
            => "Age_MaxMemberLimitBySuperCheck";

        public override bool IsValid()
            => AgentId > 0;
    }
}

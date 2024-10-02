namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetSubGroupListCriteria : CriteriaBase
    {
        public GetSubGroupListCriteria(int groupId, string subGroup)
        {
            GroupId = groupId;
            SubGroup = subGroup;
        }

        public string SubGroup { get; }

        public int GroupId { get; }

        public override string GetSettingKey() => "EAS_GetSubGroupList";

        public override bool IsValid() => true;
    }
}

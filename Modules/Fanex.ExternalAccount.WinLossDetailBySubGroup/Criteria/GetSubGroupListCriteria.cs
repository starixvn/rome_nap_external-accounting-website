namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.Data.Repository;

    public class GetSubGroupListCriteria : INonQueryCommand
    {
        public GetSubGroupListCriteria(int groupId, string subGroup)
        {
            GroupId = groupId;
            SubGroup = subGroup;
        }

        public string SubGroup { get; }

        public int GroupId { get; }

        public string GetSettingKey() => "EAS_GetSubGroupList";

        public bool IsValid() => true;
        
        
        public string Subgrouplist { get; set; }
    }
}

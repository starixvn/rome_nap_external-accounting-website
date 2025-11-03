namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.Data.Repository;

    public class GetShowProductsCriteria : INonQueryCommand
    {
        public GetShowProductsCriteria(int? groupId)
        {
            GroupId = groupId;
        }

        public int? GroupId { get; }

        public string Products { get; set; }

        public string GetSettingKey() => "EAS_GetProductID_ByGroupConfiguration";

        public bool IsValid() => true;
    }
}
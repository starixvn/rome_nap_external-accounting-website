namespace Fanex.ProductStatus.Setting.Models
{
    public class ProductGroupProfile
    {
        public ProductGroupProfile()
        {

        }
        public ProductGroupProfile(int groupId, int groupTypeId)
        {
            GroupId = groupId;
            GroupTypeId = groupTypeId;
        }

        public ProductGroupProfile(int groupId, int groupTypeId, byte settingLevel)
        {
            GroupId = groupId;
            GroupTypeId = groupTypeId;
            SettingLevel = settingLevel;
        }

        public int GroupId { get; }

        public int GroupTypeId { get; }

        public byte SettingLevel { get; }
    }
}

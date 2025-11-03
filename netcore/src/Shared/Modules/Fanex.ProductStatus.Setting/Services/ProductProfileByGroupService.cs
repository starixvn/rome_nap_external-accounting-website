namespace Fanex.ProductStatus.Setting.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Fanex.ProductStatus.Setting.Configuration;
    using Fanex.ProductStatus.Setting.Criteria;
    using Fanex.ProductStatus.Setting.Models;
    using Fanex.ProductStatus.Setting.Shared;

    internal interface IProductProfileByGroupService
    {
        int GetSettingLevel(int groupId, int systemId, byte settingStatus);
    }

    public class ProductProfileByGroupService : IProductProfileByGroupService
    {
        private static IEnumerable<ProductGroupProfile> DCSProductSetting = new List<ProductGroupProfile>
        {
            new ProductGroupProfile(2, 160),
            new ProductGroupProfile(3, 160)
        };

        private readonly ProductGroupProfileDataAccess productGroupProfileDataAccess;

        public ProductProfileByGroupService(ProductGroupProfileDataAccess productGroupProfileDataAccess = null)
        {
            this.productGroupProfileDataAccess = productGroupProfileDataAccess ?? new ProductGroupProfileDataAccessImpl();
        }

        public int GetSettingLevel(int groupId, int systemId, byte settingStatus)
        {
            var productProfile = productGroupProfileDataAccess.GetProductGroupProfile(
                new SelectProductProfileByGroupCriteria(
                    groupId,
                    DCSProductSetting.First(i => i.GroupId == groupId).GroupTypeId,
                    systemId,
                    settingStatus));

            return productProfile.SettingLevel;
        }
    }
}
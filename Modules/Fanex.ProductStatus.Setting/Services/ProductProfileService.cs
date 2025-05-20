namespace Fanex.ProductStatus.Setting.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Fanex.ProductStatus.Setting.Configuration;
    using Fanex.ProductStatus.Setting.Criteria;
    using Fanex.ProductStatus.Setting.Models;
    using Fanex.ProductStatus.Setting.Shared;

    internal interface IProductProfileService
    {
        int GetSettingLevel(int productId, int systemId);
    }

    public class ProductProfileService : IProductProfileService
    {
        private readonly IProductStatusActor productStatusActor;

        private static IEnumerable<ProductProfile> DCSProductSetting = new List<ProductProfile>
        {
            new ProductProfile(ProductIdentity.Sportsbook, null, 13),
            new ProductProfile(ProductIdentity.Casino, null, 13),
            new ProductProfile(ProductIdentity.VirtualSports, null, 13),
            new ProductProfile(ProductIdentity.LiveCasino, null, 13),
            new ProductProfile(ProductIdentity.RNGSlot, null, 21),
            new ProductProfile(ProductIdentity.RNGLottery, null, 13),
            new ProductProfile(ProductIdentity.VirtualGames, null, 13),
            new ProductProfile(ProductIdentity.Sabaclub, null, 13),
            new ProductProfile(ProductIdentity.SabaVirtualSports, null, 4),
            new ProductProfile(ProductIdentity.VGaming, null, 13),
            new ProductProfile(ProductIdentity.BitcoinBetting, null, 13),
            new ProductProfile(ProductIdentity.Cashout, null, 4),
        };

        private readonly ProductProfileDataAccess productProfileDataAccess;

        public ProductProfileService(ProductProfileDataAccess productProfileDataAccess = null, IProductStatusActor productStatusActor = null)
        {
            this.productProfileDataAccess = productProfileDataAccess ?? new ProductProfileDataAccessImpl();
            this.productStatusActor = productStatusActor ?? ProductStatusConfigurationHandler.Actor;
        }

        public int GetSettingLevel(int productId, int systemId)
        {
            var productProfile = productProfileDataAccess.GetProductProfile(
                new SelectProductProfileCriteria(
                    productId,
                    DCSProductSetting.First(i => i.ProductId == productId).SettingType.Value,
                    systemId,
                    productStatusActor.SettingStatus));

            return productProfile.SettingLevel.Value;
        }
    }
}
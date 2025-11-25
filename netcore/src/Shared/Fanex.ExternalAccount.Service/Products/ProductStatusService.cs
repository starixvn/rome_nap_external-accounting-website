namespace Fanex.ExternalAccount.Service.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Products;
    using DTO.Products;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Utilities;

    public interface IProductStatusService
    {
        ProductStatusDto GetProductStatus(int custId, int roleId, int productId, byte settingStatus);

        List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus);

        IEnumerable<Product> GetVisibleProductGroups(int custId, int roleId, byte settingStatus);
    }

    public class ProductStatusServiceImpl : IProductStatusService
    {
        private readonly ProductStatusDataAccess dataAccess;

        public ProductStatusServiceImpl(ProductStatusDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new ProductStatusDataAccessImpl();
        }

        public ProductStatusDto GetProductStatus(int custId, int roleId, int productId, byte settingStatus)
        {
            var productStatusList = this.dataAccess.GetProductStatusList(custId, roleId, settingStatus);

            var productStatus = productStatusList.FirstOrDefault(product => product.ProductId == productId);

            return productStatus ?? new ProductStatusDto { IsHidden = true };
        }

        public List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus)
            => this.dataAccess.GetProductStatusList(custId, roleId, settingStatus);

        public IEnumerable<Product> GetVisibleProductGroups(int custId, int roleId, byte settingStatus)
        {
            var productStatusList = GetProductStatusList(custId, roleId, Config.SettingStatus);
            var visibledProductGroups = productStatusList
                .Where(p => p.GroupId != null && !p.IsHidden)
                .Select(p => p.GroupId.Value)
                .Distinct()
                .Select(groupId => new Product(groupId, groupId, CacheRef.GetProductGroupName(groupId)));

            return visibledProductGroups;
        }
    }
}
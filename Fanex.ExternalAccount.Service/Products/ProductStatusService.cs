namespace Fanex.ExternalAccount.Service.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.Products;
    using DTO.Products;

    public interface ProductStatusService
    {
        ProductStatusDto GetProductStatus(int custId, int roleId, int productId);

        List<ProductStatusDto> GetProductStatusList(int custId, int roleId);
    }

    public class ProductStatusServiceImpl : ProductStatusService
    {
        private readonly ProductStatusDataAccess dataAccess;

        public ProductStatusServiceImpl(ProductStatusDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new ProductStatusDataAccessImpl();
        }

        public ProductStatusDto GetProductStatus(int custId, int roleId, int productId)
        {
            var productStatusList = this.dataAccess.GetProductStatusList(custId, roleId);

            var productStatus = productStatusList.FirstOrDefault(product => product.ProductId == productId);

            return productStatus ?? new ProductStatusDto { IsHidden = true };
        }

        public List<ProductStatusDto> GetProductStatusList(int custId, int roleId)
            => this.dataAccess.GetProductStatusList(custId, roleId);
    }
}
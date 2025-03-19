namespace Fanex.ExternalAccount.DataAccess.Products
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using DTO.Products;
    using System.Linq;

    public interface ProductStatusDataAccess
    {
        List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus);
    }

    public class ProductStatusDataAccessImpl : ProductStatusDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProductStatusDataAccessImpl() : this(null)
        {
        }

        public ProductStatusDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public List<ProductStatusDto> GetProductStatusList(int custId, int roleId, byte settingStatus)
        {
            var criteria = new ProductStatusCriteria(custId, roleId, settingStatus);
            var dbResult = dynamicRepository.Fetch<ProductStatusDto>(criteria);
            return dbResult.ToList();
        }
    }

    // Local criteria class to avoid creating new file
}
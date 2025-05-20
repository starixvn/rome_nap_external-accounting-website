namespace Fanex.ProductStatus.Setting.DataAccess
{
    using Criteria;
    using Fanex.Data.Repository;
    using Models;

    public interface ProductProfileDataAccess
    {
        ProductProfile GetProductProfile(SelectProductProfileCriteria criteria);
    }

    public class ProductProfileDataAccessImpl : ProductProfileDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProductProfileDataAccessImpl() : this(null)
        {
        }

        public ProductProfileDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public ProductProfile GetProductProfile(SelectProductProfileCriteria criteria)
            => dynamicRepository.Get<ProductProfile>(criteria);
    }
}

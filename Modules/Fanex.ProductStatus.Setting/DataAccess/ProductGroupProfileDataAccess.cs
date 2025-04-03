namespace Fanex.ProductStatus.Setting.DataAccess
{
    using Criteria;
    using Fanex.Data.Repository;
    using Models;

    public interface ProductGroupProfileDataAccess
    {
        ProductGroupProfile GetProductGroupProfile(SelectProductProfileByGroupCriteria criteria);
    }

    public class ProductGroupProfileDataAccessImpl : ProductGroupProfileDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProductGroupProfileDataAccessImpl() : this(null)
        {
        }

        public ProductGroupProfileDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public ProductGroupProfile GetProductGroupProfile(SelectProductProfileByGroupCriteria criteria)
            => dynamicRepository.Get<ProductGroupProfile>(criteria);
    }
}

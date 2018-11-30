namespace Fanex.ExternalAccount.Service.Products
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using DTO.Products;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.NAP.Framework.Data;

    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProducts();
    }

    public class ProductService : IProductService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICache Cache = CacheService.CreateInstance(CacheType.MemoryCache);

        public ProductService() : this(null)
        {

        }

        public ProductService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository?? new DynamicRepository();
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            var key = "Fanex.NAP.External.Site.AllProducts";
            var result = Cache.Get<IEnumerable<ProductDto>>(key);

            if (result == null)
            {
                result = dynamicRepository.Fetch<ProductDto>(new GetProductsCriteria());
                    Cache.Set<IEnumerable<ProductDto>>(key, result, DateTime.Now.AddDays(1));
            }

            return result;
        }
            
    }

}

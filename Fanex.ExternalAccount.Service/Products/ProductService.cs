namespace Fanex.ExternalAccount.Service.Products
{
    using DTO.Products;
    using Fanex.ExternalAccount.DataAccess.Products;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data.Repository;

    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProducts();

        IEnumerable<ProductDto> GetProducts(string groupName, int groupId);

        IEnumerable<int> ValidateProductIds(string groupName, int groupId, IEnumerable<int> productIds);
    }

    public class ProductService : IProductService
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProductService(
            IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            var products = GetRawProducts(string.Empty, null);

            return GroupProducts(products);
        }

        public IEnumerable<ProductDto> GetProducts(string groupName, int groupId)
        {
            var products = GetRawProducts(groupName, groupId);
            return GroupProducts(products);
        }

        private IEnumerable<ProductEntity> GetRawProducts(string groupName, int? groupId)
        {
            return dynamicRepository.Fetch<ProductEntity>(new GetProductsCriteria(groupName,
                (groupId == null) ? null : GetShowProducts(groupId)));
        }

        private string GetShowProducts(int? GroupId = null)
        {
            var criteria = new GetShowProductsCriteria(GroupId);
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var ouputParams = db.GetOuputParams();
            //     return ouputParams["products"] == null ? string.Empty : ouputParams["products"].ToString();
            // }

            dynamicRepository.Execute(criteria);
            return criteria.Products ?? string.Empty;

        }

        private static IEnumerable<ProductDto> GroupProducts(IEnumerable<ProductEntity> products)
        {
            var productList = new List<ProductDto>();

            products.GroupBy(p => p.Type)
                .ToList()
                .ForEach(grp =>
                {
                    if (grp.Key == 0)
                    {
                        productList.AddRange(grp.Select(p => new ProductDto(p.ProductId, p.ProductName)));
                    }
                    else
                    {
                        productList.Add(new ProductDto(grp.Key,
                            products: grp.Select(p => new ProductDto(p.ProductId, p.ProductName))));
                    }
                });

            return productList;
        }

        public IEnumerable<int> ValidateProductIds(string groupName, int groupId, IEnumerable<int> productIds)
        {
            if (!productIds.Any())
            {
                return Enumerable.Empty<int>();
            }

            var productIdsHash = ToDictionary(productIds);
            var products = GetRawProducts(groupName, groupId);

            return products
                .Where(p => productIdsHash.ContainsKey(p.ProductId))
                .Select(p => p.ProductId);
        }

        private static IDictionary<int, bool> ToDictionary(IEnumerable<int> arrayInt)
        {
            var dictionary = new Dictionary<int, bool>();

            foreach (var item in arrayInt)
            {
                dictionary[item] = true;
            }

            return dictionary;
        }
    }
}
namespace Fanex.ExternalAccount.DTO.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.ExternalAccount.Core.Shared;

    public class ProductDto
    {
        public ProductDto(int productId, string productName = null, IEnumerable<ProductDto> products = null)
        {
            ProductId = productId;
            ProductName = CacheRef.GetProductName(productId, productName);
            Products = products;
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public IEnumerable<ProductDto> Products { get; }

        public bool IsParent => Products?.Any() == true;
    }
}
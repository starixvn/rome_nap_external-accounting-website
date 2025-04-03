namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Services;
    using Shared;

    public class MultipleQueryViewModel : BaseQueryViewModel
    {
        private readonly IEnumerable<Product> customerProductList;

        private IEnumerable<Product> products;

        public MultipleQueryViewModel()
            : this(null, null)
        {
        }

        public MultipleQueryViewModel(
            IEnumerable<Product> customerProductList,
            IProductStatusActor productStatusActor)
            : base(productStatusActor)
        {
            this.customerProductList = customerProductList
                ?? ProductStatusConfigurationHandler.CustomerProductListGetter();
        }

        public string ProductIds { get; set; }

        public string CustIds { get; set; }

        public string CustNames { get; set; }

        public IEnumerable<Product> Products
        {
            get
            {
                if (products == null)
                {
                    products = customerProductList;

                    if (!string.IsNullOrWhiteSpace(ProductIds))
                    {
                        var includedProducts = ProductIds.ToDictionary();
                        products = products.Where(p => includedProducts.ContainsKey(p.Id));
                    }
                }

                return products;
            }
        }
    }
}
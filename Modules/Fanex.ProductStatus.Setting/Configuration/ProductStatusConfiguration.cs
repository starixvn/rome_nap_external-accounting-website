namespace Fanex.ProductStatus.Setting.Configuration
{
    using System;
    using System.Collections.Generic;
    using Services;

    public class ProductStatusConfiguration
    {
        internal IProductStatusAuthorizeService AuthorizeService { get; private set; }

        internal IProductStatusActor Actor { get; private set; }

        internal string UnAuthorizedUrl { get; private set; }

        internal Func<IEnumerable<Product>> CustomerProductListGetter { get; private set; }

        public ProductStatusConfiguration UseAuthorizeService(IProductStatusAuthorizeService authorizeService)
        {
            AuthorizeService = authorizeService;
            return this;
        }

        public ProductStatusConfiguration UseProductStatusActor(IProductStatusActor productStatusActor)
        {
            Actor = productStatusActor;
            return this;
        }

        public ProductStatusConfiguration SetUnAuthorizedUrl(string url)
        {
            UnAuthorizedUrl = url;
            return this;
        }

        public ProductStatusConfiguration UseCustomerProductList(Func<IEnumerable<Product>> customerProductListGetter)
        {
            CustomerProductListGetter = customerProductListGetter;
            return this;
        }
    }
}
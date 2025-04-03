namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Models;
    using Services;
    using Shared;

    public class QueryViewModel : BaseQueryViewModel
    {
        public QueryViewModel() 
            : this(null)
        {

        }

        public QueryViewModel(IProductStatusActor productStatusActor)
            : base(productStatusActor)
        {
        }

        public string CustName { get; set; }

        public bool HasData { get; private set; }

        public bool IsMultipleBetTypes { get; private set; }

        public bool AllowUpdate { get; private set; }

        public IDictionary<byte, bool> ShowColumnByRoles { get; private set; }
            = new Dictionary<byte, bool>
            {
                [RoleIdentity.Master] = false,
                [RoleIdentity.Agent] = false,
                [RoleIdentity.Member] = false
            };

        public override BaseQueryViewModel SetProductStatusViewModel(IEnumerable<ProductStatusModel> models)
        {
            base.SetProductStatusViewModel(models);
            HasData = ProductStatus.Any();

            if (HasData)
            {
                IsMultipleBetTypes = ProductStatus.Count() > 1;
                AllowUpdate = ProductStatus.Any(product => !product.IsDisabledUpline);

                if (HasData)
                {
                    var firstProductStatus = ProductStatus.First();
                    ShowColumnByRoles[RoleIdentity.Master] = firstProductStatus.IsShowMaster;
                    ShowColumnByRoles[RoleIdentity.Agent] = firstProductStatus.IsShowAgent;
                    ShowColumnByRoles[RoleIdentity.Member] = firstProductStatus.IsShowMember;
                }
            }

            return this;
        }
    }
}

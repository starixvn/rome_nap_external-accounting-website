namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using AppResources;
    using Configuration;
    using Models;
    using Services;
    using Shared;

    public class SimpleQueryViewModel
    {
        private readonly IDictionary<byte, string> roleDictionary = new Dictionary<byte, string>
        {
            [RoleIdentity.Super] = Resources.Super,
            [RoleIdentity.Master] = Resources.Master,
            [RoleIdentity.Agent] = Resources.Agent,
            [RoleIdentity.Member] = Resources.Member,
        };

        private readonly IProductStatusActor productStatusActor;

        public SimpleQueryViewModel() 
            : this(null)
        {

        }

        public SimpleQueryViewModel(IProductStatusActor productStatusActor)
        {
            this.productStatusActor = productStatusActor ?? ProductStatusConfigurationHandler.Actor;
        }

        public byte SystemId
            => productStatusActor.SystemId;

        public byte SettingStatus
            => productStatusActor.SettingStatus;

        public int ProductId { get; set; }

        public int CustId { get; set; }

        public int SportTypeId { get; set; }

        public byte CustRole { get; set; }

        public string CustName { get; set; }

        public IEnumerable<SimpleProductStatusViewModel> ProductStatus { get; private set; }
            = Enumerable.Empty<SimpleProductStatusViewModel>();

        public bool HasData { get; private set; }

        public bool IsMultipleBetTypes { get; private set; }

        public bool AllowUpdate { get; private set; }

        public SimpleQueryViewModel SetSimpleProductStatusViewModel(IEnumerable<ProductStatusModel> models)
        {
            var dataCount = models.Count();
            HasData = dataCount > 0;

            if (HasData)
            {
                IsMultipleBetTypes = dataCount > 1;
                ProductStatus = models
                    .Select(status => new SimpleProductStatusViewModel(status))
                    .ToList();

                AllowUpdate = ProductStatus.Any(product => !product.IsDisabledUpline);
            }

            return this;
        }

        public string GetRoleName()
        {
            string roleName;
            roleDictionary.TryGetValue(CustRole, out roleName);

            return roleName;
        }

        public bool IsValid()
            => ProductId > 0
            && CustId > 0
            && roleDictionary.ContainsKey(CustRole);
    }
}

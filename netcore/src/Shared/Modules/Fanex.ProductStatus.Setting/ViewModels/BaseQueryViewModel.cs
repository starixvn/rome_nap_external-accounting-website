namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using AppResources;
    using Configuration;
    using Models;
    using Services;
    using Shared;

    public class BaseQueryViewModel
    {
        private readonly IProductStatusActor productStatusActor;

        protected readonly IDictionary<short, string> roleDictionary = new Dictionary<short, string>
        {
            [RoleIdentity.Super] = Resources.Super,
            [RoleIdentity.SuperDirectMember] = Resources.SuperDirectMember,
            [RoleIdentity.Master] = Resources.Master,
            [RoleIdentity.MasterDirectMember] = Resources.MasterDirectMember,
            [RoleIdentity.Agent] = Resources.Agent,
            [RoleIdentity.Member] = Resources.Member,
        };

        public BaseQueryViewModel(IProductStatusActor productStatusActor = null)
        {
            this.productStatusActor = productStatusActor ?? ProductStatusConfigurationHandler.Actor;
        }

        public bool IsGroup { get; set; }

        public int ProductId { get; set; }

        public int CustId { get; set; }

        public int SportTypeId { get; set; }

        public short CustRole { get; set; }

        public short Recommend { get; set; }

        public byte SystemId
            => productStatusActor.SystemId;

        public byte SettingStatus
            => productStatusActor.SettingStatus;

        public IEnumerable<ProductStatusViewModel> ProductStatus { get; private set; }
            = Enumerable.Empty<ProductStatusViewModel>();

        public string GetRoleName(short roleId)
        {
            string roleName;
            roleDictionary.TryGetValue(roleId, out roleName);

            return roleName;
        }

        public virtual BaseQueryViewModel SetProductStatusViewModel(IEnumerable<ProductStatusModel> models)
        {
            ProductStatus = models
                .Select(status => new ProductStatusViewModel(status, CustRole))
                .ToList();

            return this;
        }

        public bool IsValid()
            => ProductId > 0
            && CustId > 0
            && roleDictionary.ContainsKey(CustRole);
    }
}
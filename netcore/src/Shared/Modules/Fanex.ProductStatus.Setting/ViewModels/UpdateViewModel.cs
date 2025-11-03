namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Models;
    using Services;
    using Shared;

    public class UpdateViewModel
    {
        private readonly IProductStatusActor productStatusActor;

        public UpdateViewModel()
            : this(null)
        {
        }

        public UpdateViewModel(IProductStatusActor productStatusActor)
        {
            this.productStatusActor = productStatusActor ?? ProductStatusConfigurationHandler.Actor;
        }

        public bool IsGroup { get; set; }

        public int ProductId { get; set; }

        public int CustId { get; set; }

        public byte CustRole { get; set; }

        public IEnumerable<ProductStatusCommand> ProductStatus { get; set; }

        public string IP
            => productStatusActor.ClientIP;

        public int ActorId
            => productStatusActor.Id;

        public string ActorName
           => productStatusActor.Name;

        public byte SystemId
           => productStatusActor.SystemId;

        public byte SettingStatus
           => productStatusActor.SettingStatus;

        public virtual bool IsValid()
            => ProductId > 0
            && CustId > 0
            && RoleIdentity.Member <= CustRole && CustRole <= RoleIdentity.Super
            && ActorId > 0
            && ProductStatus.Any();
    }
}
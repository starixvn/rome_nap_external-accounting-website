namespace Fanex.ProductStatus.Setting.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Models;
    using Services;
    using Shared;

    public class MultipleUpdateViewModel
    {
        private readonly IProductStatusActor productStatusActor;

        private int[] customerIds;
        private string[] customerNames;

        public MultipleUpdateViewModel()
            : this(null)
        {
        }

        public MultipleUpdateViewModel(IProductStatusActor productStatusActor)
        {
            this.productStatusActor = productStatusActor ?? ProductStatusConfigurationHandler.Actor;
        }

        public bool IsGroup { get; set; }

        public byte CustRole { get; set; }

        public int ProductId { get; set; }

        public string CustIds { get; set; }

        public string CustNames { get; set; }

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

        public int[] CustomerIds
        {
            get
            {
                customerIds = customerIds ?? CustIds.SplitInt();

                return customerIds;
            }
        }

        public string[] CustomerNames
        {
            get
            {
                customerNames = customerNames ?? CustNames.SplitString();

                return customerNames;
            }
        }

        public bool IsValid()
        {
            var isValidCustomers = CustomerIds.Length > 0
                && CustomerNames.Length > 0
                && CustomerIds.Length == CustomerNames.Length;

            return ProductId > 0
                && isValidCustomers
                && RoleIdentity.Member <= CustRole && CustRole <= RoleIdentity.Super
                && ProductStatus.Any();
        }
    }
}
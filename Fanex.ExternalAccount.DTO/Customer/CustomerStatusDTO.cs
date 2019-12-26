using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public class CustomerStatusDTO
    {
        public bool SyncedCasino { get; set; }

        public bool UplineSyncedCasino { get; set; }

        public bool DisableCasino { get; set; }

        public bool HideVirtualSport { get; set; }

        public bool DisableVirtualSport { get; set; }

        public bool HideKeno { get; set; }

        public bool DisableKeno { get; set; }

        public bool HideGoldDeluxe { get; set; }

        public bool UplineHideGoldDeluxe { get; set; }

        public bool DisableGoldDeluxe { get; set; }

        public bool DisableTransfer { get; set; }

        public bool HasSecurityCode { get; set; }

        public bool UplineHasSecurityCode { get; set; }

        public List<CustomerStatusDCS> DCSProductsSetting { get; set; }

        public CustomerStatusDTO()
        {
            DCSProductsSetting = new List<CustomerStatusDCS>();
        }

        public bool IsDCSProductExisted(int productId)
        {
            foreach(CustomerStatusDCS product in DCSProductsSetting)
            {
                if (product.ProductID == productId)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class CustomerStatusDCS
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public bool IsHidden { get; set; }

        public bool IsSynced { get; set; }

        public bool IsSyncedUpline { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDisabledUpline { get; set; }
    }
}

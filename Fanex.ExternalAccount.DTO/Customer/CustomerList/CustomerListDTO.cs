namespace Fanex.ExternalAccount.DTO.Customer.CustomerList
{
    public class CustomerListDTO
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public string UserName { get; set; }

        public bool Enabled { get; set; }

        public bool Closed { get; set; }

        public byte ClosedReason { get; set; }

        public byte Locked { get; set; }


        public CustomerLockedAccess LockedAccess => new CustomerLockedAccess(this.Locked);

        public bool Suspended { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CreateDate { get; set; }

        public string LastLoginIP { get; set; }

        public bool UplineDoubleCommLosing { get; set; }

        public bool DoubleCommLosing { get; set; }

        public int NoOfDownlines { get; set; }

        public int? LimitDays { get; set; }

        public string MinDate { get; set; }

        public decimal GroupA { get; set; }

        public decimal GroupB { get; set; }

        public decimal GroupC { get; set; }

        public decimal GroupD { get; set; }

        public string UserGroup { get; set; }

        public string NickName { get; set; }

        public decimal Discount1x2 { get; set; }

        public decimal DiscountCS { get; set; }

        public decimal Discount { get; set; }

        public decimal PlayerDiscount1x2 { get; set; }

        public decimal PlayerDiscountCS { get; set; }

        public decimal PlayerDiscount { get; set; }

        public string SubGroup { get; set; }

        public string Currency { get; set; }

        public int ServiceType { get; set; }
        
        public bool IsHideIP { get; set; }
    }
}

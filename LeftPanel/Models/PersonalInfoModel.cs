//namespace Fanex.ExternalAccount.Module.LeftPanel.Models
//{
//    using Fanex.ExternalAccount.DTO.Customer;

//    public class PersonalInfoModel
//    {
//        public int CustId { get; set; }

//        public string Fax { get; set; }

//        public string MobilePhone { get; set; }

//        public string FirstName { get; set; }

//        public string LastName { get; set; }

//        public string Name => $"{FirstName} {LastName}";

//        // Username2 is a nickname
//        public string Nickname { get; set; }

//        public string Phone { get; set; }

//        public string Headline { get; set; }

//        public byte ClosedReason { get; set; }

//        public CustomerClosedReason CustomerClosedReason => new CustomerClosedReason(ClosedReason);

//        public bool HasDownline { get; set; }

//        public bool UplineHasSecurityCode { get; set; }

//        // Credit is needed to be Formatted to 2 decimal places.
//        public decimal Credit { get; set; }

//        // RewardReceivable is needed to be Formatted to 2 decimal places.
//        public decimal RewardReceivable { get; set; }

//        // RewardPayable is needed to be Formatted to 2 decimal places.
//        public decimal RewardPayable { get; set; }

//        public string RewardCurrency { get; set; }

//        public string StatusTooltip(bool enabled, bool closed, bool suspended, byte locked)
//        {
//            if (!enabled)
//            {
//                return "This account was disabled.";
//            }

//            if (closed)
//            {
//                var customerClosedReason = CustomerClosedReason;
//                return customerClosedReason.ClosedReason;
//            }

//            if (suspended)
//            {
//                return "This account was suspended.";
//            }

//            var lockedAccess = new CustomerLockedAccess(locked);

//            return lockedAccess.IsLocked ? lockedAccess.Reason : string.Empty;
//        }

//        public string StatusBackground(bool enabled, bool closed, bool suspended, byte locked)
//        {
//            if (!enabled)
//            {
//                return "disabled";
//            }

//            if (closed)
//            {
//                return CustomerClosedReason.ClassName;
//            }

//            if (suspended)
//            {
//                return "suspended";
//            }

//            var lockedAccess = new CustomerLockedAccess(locked);

//            return lockedAccess.IsLocked ? lockedAccess.ClassName : string.Empty;
//        }
//    }
//}

namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using DTO.Customer;
    using Utilities;

    public static class PersonalInfoDataRowExtensions
    {
        internal static int CustId(this DataRow dataRow) => Utils.ToInt32(dataRow["custid"]);

        internal static string Fax(this DataRow dataRow) => Utils.ToString(dataRow["fax"]);

        internal static string MobilePhone(this DataRow dataRow) => Utils.ToString(dataRow["mobilephone"]);

        internal static string Name(this DataRow dataRow) => Utils.ToString(dataRow["firstname"]) + " " + Utils.ToString(dataRow["lastname"]);

        internal static string Nickname(this DataRow dataRow) => Utils.ToString(dataRow["username2"]);

        internal static string Phone(this DataRow dataRow) => Utils.ToString(dataRow["Phone"]);

        internal static string Headline(this DataRow dataRow) => Utils.ToString(dataRow["Headline"]);

        internal static CustomerClosedReason CustomerClosedReason(this DataRow dataRow) => new CustomerClosedReason(dataRow["closedreason"]);

        internal static bool HasDownline(this DataRow dataRow) => Utils.ToBoolean(dataRow["HasDownline"]);

        internal static bool UplineHasSecurityCode(this DataRow dataRow) => Utils.ToBoolean(dataRow["UplineHasSecurityCode"]);

        internal static decimal Credit(this DataRow dataRow) => Utils.ToDecimal(dataRow["credit"]).Format();
        
        internal static decimal RewardReceivable(this DataRow dataRow) => Utils.ToDecimal(dataRow["rewardreceivable"]).Format();
        
        internal static decimal RewardPayable(this DataRow dataRow) => Utils.ToDecimal(dataRow["rewardpayable"]).Format();
        
        internal static string RewardCurrency(this DataRow dataRow) => Utils.ToString(dataRow["rewardcurrency"]);

        internal static string StatusTooltip(this DataRow dataRow, bool enabled, bool closed, bool suspended, byte locked)
        {
            if (!enabled)
            {
                return "This account was disabled.";
            }

            if (closed)
            {
                var customerClosedReason = dataRow.CustomerClosedReason();
                return customerClosedReason.ClosedReason;
            }          

            if (suspended)
            {
                return "This account was suspended.";
            }

            var lockedAccess = new CustomerLockedAccess(locked);

            return lockedAccess.IsLocked ? lockedAccess.Reason : string.Empty;
        }

        internal static string StatusBackground(this DataRow dataRow, bool enabled, bool closed, bool suspended, byte locked)
        {
            if (!enabled)
            {
                return "disabled";
            }

            if (closed)
            {
                return dataRow.CustomerClosedReason().ClassName;
            }

            if (suspended)
            {
                return "suspended";
            }

            var lockedAccess = new CustomerLockedAccess(locked);

            return lockedAccess.IsLocked ? lockedAccess.ClassName : string.Empty;
        }
    }
}

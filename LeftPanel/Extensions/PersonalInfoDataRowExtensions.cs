namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using DTO.Customer;
    using Utilities;

    public static class PersonalInfoDataRowExtensions
    {
        private const string ThisAccountWas = "This account was ";

        internal static int CustId(this DataRow dataRow) => Utils.ToInt32(dataRow["custid"]);

        internal static string Fax(this DataRow dataRow) => Utils.ToString(dataRow["fax"]);

        internal static string MobilePhone(this DataRow dataRow) => Utils.ToString(dataRow["mobilephone"]);

        internal static string Name(this DataRow dataRow) => Utils.ToString(dataRow["firstname"]) + " " + Utils.ToString(dataRow["lastname"]);

        internal static string Nickname(this DataRow dataRow) => Utils.ToString(dataRow["username2"]);

        internal static string Phone(this DataRow dataRow) => Utils.ToString(dataRow["Phone"]);

        internal static CustomerClosedReason CustomerClosedReason(this DataRow dataRow) => new CustomerClosedReason(dataRow["closedreason"]);

        internal static bool HasDownline(this DataRow dataRow) => Utils.ToBoolean(dataRow["HasDownline"]);

        internal static bool UplineHasSecurityCode(this DataRow dataRow) => Utils.ToBoolean(dataRow["UplineHasSecurityCode"]);

        internal static decimal Credit(this DataRow dataRow) => Utils.ToDecimal(dataRow["credit"]);

        internal static string StatusTooltip(this DataRow dataRow, bool enabled, bool closed, bool suspended)
        {
            var status = ThisAccountWas;

            if (closed)
            {
                var customerClosedReason = dataRow.CustomerClosedReason();
                status = customerClosedReason.ClosedReason;
            }

            if (!enabled)
            {
                return status + "disabled";
            }

            if (closed)
            {
                return status;
            }

            if (suspended)
            {
                return status + "suspended";
            }

            return string.Empty;
        }

        internal static string StatusBackground(this DataRow dataRow, bool enabled, bool closed, bool suspended)
        {
            var closedReason = string.Empty;

            if (closed)
            {
                var customerClosedReason = dataRow.CustomerClosedReason();
                closedReason = customerClosedReason.ClassName;
            }

            return !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty;
        }
    }
}

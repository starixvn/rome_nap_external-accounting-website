namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using DTO.Customer;
    using Utilities;

    public static class PersonalInfoExtensions
    {
        private const string ThisAccountWas = "This account was ";
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";

        internal static int CustId(this DataRow dataRow) => Utils.ToInt32(dataRow["custid"]);

        internal static string Fax(this DataRow dataRow) => Utils.ToString(dataRow["fax"]);

        internal static string MobilePhone(this DataRow dataRow) => Utils.ToString(dataRow["mobilephone"]);

        internal static string Name(this DataRow dataRow) => Utils.ToString(dataRow["firstname"]) + " " + Utils.ToString(dataRow["lastname"]);

        internal static string Nickname(this DataRow dataRow) => Utils.ToString(dataRow["username2"]);

        internal static string Phone(this DataRow dataRow) => Utils.ToString(dataRow["Phone"]);

        internal static bool IsInternal(this DataRow dataRow) => Utils.ToBoolean(dataRow["isinternal"]);

        internal static CustomerClosedReason CustomerClosedReason(this DataRow dataRow) => new CustomerClosedReason(dataRow["closedreason"]);

        internal static bool HasDownline(this DataRow dataRow) => Utils.ToBoolean(dataRow["HasDownline"]);

        internal static bool UplineHasSecurityCode(this DataRow dataRow) => Utils.ToBoolean(dataRow["UplineHasSecurityCode"]);

        internal static int CurrencyId(this DataRow dataRow) => Utils.ToInt32(dataRow["exchangeid"]);

        internal static int Currency(this DataRow dataRow) => Utils.ToInt32(dataRow["currency"]);

        internal static string CreateDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["createdate"]).ToStringExt(DateTimeFormat);

        internal static string LastLogged(this DataRow dataRow) => Utils.ToDateTime(dataRow["lastlogged"]).ToStringExt(DateTimeFormat);

        internal static string LastTxn(this DataRow dataRow) => Utils.ToDateTime(dataRow["lastTxn"]).ToStringExt(DateTimeFormat);

        internal static string ExpiredPasswordDay(this DataRow dataRow) => Utils.ToDateTime(dataRow["pwdexpiry"]).ToStringExt(DateTimeFormat);

        internal static string LastChangedSecurityCodeDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat);

        internal static string LoginOptions(this DataRow dataRow) => Utils.ToBoolean(dataRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname";

        internal static string OTPCreateDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["OTPCreateDate"]).ToStringExt(DateTimeFormat);

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

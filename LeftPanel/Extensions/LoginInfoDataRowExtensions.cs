namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class LoginInfoDataRowExtensions
    {
        internal static string CreatedDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["creatdate"]).ToStringExt();

        internal static string LastLoggedIn(this DataRow dataRow) => Utils.ToDateTime(dataRow["lastlogged"]).ToStringExt();

        internal static string LastLoginIP(this DataRow dataRow) => Utils.ToString(dataRow["lastloginip"]);

        internal static string PasswordExpired(this DataRow dataRow) => Utils.ToDateTime(dataRow["pwdexpiry"]).ToStringExt();

        internal static string LastChangedSecurityCodeDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["LastChangedSecurityCodeDate"]).ToStringExt();

        internal static string LoginOptions(this DataRow dataRow) => Utils.ToBoolean(dataRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname";

        internal static string OTPCreateDate(this DataRow dataRow) => Utils.ToDateTime(dataRow["OTPCreateDate"]).ToStringExt();

        internal static string LastTxn(this DataRow dataRow) => Utils.ToDateTime(dataRow["LastTxn"]).ToStringExt();
    }
}

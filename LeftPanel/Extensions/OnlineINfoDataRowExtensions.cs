namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class OnlineInfoDataRowExtensions
    {
        internal static string ReloadTime(this DataRow dataRow) => Utils.ToDateTime(dataRow["reload_time"]).ToStringExt();

        internal static string IP(this DataRow dataRow) => dataRow.ToString("ip");

        internal static string LastLogInIP(this DataRow dataRow) => dataRow.ToString("lastloginip");
    }
}

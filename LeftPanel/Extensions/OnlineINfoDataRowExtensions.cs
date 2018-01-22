namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class OnlineInfoDataRowExtensions
    {
        internal static string ReloadTime(this DataRow dataRow) => Utils.ToDateTime(dataRow["reload_time"]).ToStringExt();
    }
}

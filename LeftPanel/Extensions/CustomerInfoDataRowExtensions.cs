namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class CustomerInfoDataRowExtensions
    {
        internal static int RoleId(this DataRow dataRow) => Utils.ToInt32(dataRow["roleid"]);

        internal static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        internal static bool Enabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["enabled"]);

        internal static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        internal static bool IsHidden(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsHidden"]);

        internal static bool IsSync(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsSync"]);

        internal static bool IsDisabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabled"]);
    }
}
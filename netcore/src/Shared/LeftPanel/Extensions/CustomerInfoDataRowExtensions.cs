namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class CustomerInfoDataRowExtensions
    {
        internal static int RoleId(this DataRow dataRow) => Utils.ToInt32(dataRow["roleid"]);

        internal static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        internal static bool ClosedUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["upLineClosed"]);

        internal static bool IsDirectClosed(this DataRow dataRow) => Utils.ToByte(dataRow["isDirectClosed"]) > 0;

        internal static byte Locked(this DataRow dataRow) => Utils.ToByte(dataRow["Locked"]);

        internal static bool IsProtectedMode(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsProtectedMode"]);

        internal static bool Enabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["enabled"]);

        internal static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        internal static bool SuspendedUpline(this DataRow dataRow) => Utils.ToBoolean(dataRow["upLineSuspend"]);

        internal static bool IsDirectSuspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDirectSuspend"]);

        internal static bool IsHidden(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsHidden"]);

        internal static bool IsSync(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsSync"]);

        internal static bool IsDisabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabled"]);

        internal static string Username(this DataRow dataRow) => dataRow["username"].ToString();

        internal static int SRecommend(this DataRow dataRow) => Utils.ToInt32(dataRow["srecommend"]);

        internal static int MRecommend(this DataRow dataRow) => Utils.ToInt32(dataRow["mrecommend"]);

        internal static int Recommend(this DataRow dataRow) => Utils.ToInt32(dataRow["recommend"]);

        internal static string Site(this DataRow dataRow) => Utils.ToString(dataRow["site"]);

        internal static string SiteLabel(this DataRow dataRow) => Utils.ToString(dataRow["sitelabel"]);

        internal static int ServiceType(this DataRow dataRow) => Utils.ToInt32(dataRow["serviceType"]);
    }
}
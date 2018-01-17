namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Data;
    using Core.Shared;
    using Utilities;

    public static class CustomerInfoRowExtension
    {
        internal static int RoleId(this DataRow dataRow) => Utils.ToInt32(dataRow["roleid"]);

        internal static int SuperId(this DataRow dataRow) => Utils.ToInt32(dataRow["superID"]);

        internal static int MasterId(this DataRow dataRow) => Utils.ToInt32(dataRow["masterID"]);

        internal static int AgentId(this DataRow dataRow) => Utils.ToInt32(dataRow["agentID"]);

        internal static int CustomerId(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.SuperId();

                case Role.Master:
                    return dataRow.MasterId();

                case Role.Agent:
                    return dataRow.AgentId();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        internal static string SuperName(this DataRow dataRow) => Utils.ToString(dataRow["superName"]);

        internal static string MasterName(this DataRow dataRow) => Utils.ToString(dataRow["masterName"]);

        internal static string AgentName(this DataRow dataRow) => Utils.ToString(dataRow["agentName"]);

        internal static string MemberName(this DataRow dataRow) => Utils.ToString(dataRow["memberName"]);

        internal static string CustomerName(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.SuperName();

                case Role.Master:
                    return dataRow.MasterName();

                case Role.Agent:
                    return dataRow.AgentName();

                case Role.Member:
                    return dataRow.MemberName();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        internal static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        internal static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        internal static bool IsHidden(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsHidden"]);

        internal static bool IsSync(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsSync"]);

        internal static bool IsDisabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabled"]);
    }
}
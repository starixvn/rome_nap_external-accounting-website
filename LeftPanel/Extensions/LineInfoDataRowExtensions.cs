namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Data;
    using Core.Shared;
    using Utilities;

    public static class LineInfoDataRowExtensions
    {
        private static int SuperId(this DataRow dataRow) => Utils.ToInt32(dataRow["superID"]);

        private static int MasterId(this DataRow dataRow) => Utils.ToInt32(dataRow["masterID"]);

        private static int AgentId(this DataRow dataRow) => Utils.ToInt32(dataRow["agentID"]);

        private static int MemberId(this DataRow dataRow) => Utils.ToInt32(dataRow["memberID"]);

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
                case Role.Member:
                    return dataRow.MemberId();
                default:
                    throw new ArgumentException(nameof(role));
            }
        }

        private static string SuperName(this DataRow dataRow) => Utils.ToString(dataRow["superName"]);

        private static string MasterName(this DataRow dataRow) => Utils.ToString(dataRow["masterName"]);

        private static string AgentName(this DataRow dataRow) => Utils.ToString(dataRow["agentName"]);

        private static string MemberName(this DataRow dataRow) => Utils.ToString(dataRow["memberName"]);

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

        private static bool SuperEnabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["superEnabled"]);

        private static bool MasterEnabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["masterEnabled"]);

        private static bool AgentEnabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["agentEnabled"]);

        private static bool MemberEnabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["membeEnabled"]);

        internal static bool CustomerEnabled(this DataRow dataRow, int role)
        {
            switch (role)
            {
                case Role.Super:
                    return dataRow.SuperEnabled();

                case Role.Master:
                    return dataRow.MasterEnabled();

                case Role.Agent:
                    return dataRow.AgentEnabled();

                case Role.Member:
                    return dataRow.MemberEnabled();

                default:
                    throw new ArgumentException(nameof(role));
            }
        }
    }
}
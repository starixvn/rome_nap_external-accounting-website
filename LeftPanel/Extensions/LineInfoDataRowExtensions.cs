namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Data;
    using Core.Shared;
    using Utilities;

    public static class LineInfoDataRowExtensions
    {
        private static int SuperId(this DataRow dataRow) => Utils.ToInt32(dataRow["srecommend"]);

        private static int MasterId(this DataRow dataRow) => Utils.ToInt32(dataRow["mrecommend"]);

        private static int AgentId(this DataRow dataRow) => Utils.ToInt32(dataRow["recommend"]);

        private static int MemberId(this DataRow dataRow) => Utils.ToInt32(dataRow["custid"]);

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

        private static string MemberName(this DataRow dataRow) => Utils.ToString(dataRow["userName"]);

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
    }
}
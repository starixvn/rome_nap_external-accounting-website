namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class UsedCreditItemDataReaderExtensions
    {
        internal static string Username(this IDataReader reader) => reader.ToString("username");

        internal static string FirstName(this IDataReader reader) => reader.ToString("firstname");

        internal static string LastName(this IDataReader reader) => reader.ToString("lastname");

        internal static decimal Credit(this IDataReader reader) => reader.ToDecimal("credit");

        internal static bool IsClosed(this IDataReader reader) => reader.ToBoolean("closed");

        internal static string MasterName(this IDataReader reader) => reader.ToString("mastername");

        internal static decimal GivenCredit(this IDataReader reader) => reader.ToDecimal("givencredit");

        internal static decimal UsedCredit(this IDataReader reader) => reader.ToDecimal("usedcredit");

        internal static string AgentName(this IDataReader reader) => reader.ToString("agentname");
    }
}

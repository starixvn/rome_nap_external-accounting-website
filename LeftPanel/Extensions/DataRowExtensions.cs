namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Utilities;

    public static class DataRowExtensions
    {
        public static int SuperId(this DataRow dataRow) => Utils.ToInt32(dataRow["superID"]);

        public static int MasterId(this DataRow dataRow) => Utils.ToInt32(dataRow["masterID"]);

        public static int AgentId(this DataRow dataRow) => Utils.ToInt32(dataRow["agentID"]);

        public static string SuperName(this DataRow dataRow) => Utils.ToString(dataRow["superName"]);

        public static string MasterName(this DataRow dataRow) => Utils.ToString(dataRow["masterName"]);

        public static string AgentName(this DataRow dataRow) => Utils.ToString(dataRow["agentName"]);

        public static string MemberName(this DataRow dataRow) => Utils.ToString(dataRow["memberName"]);

        public static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        public static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        public static bool IsHidden(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsHidden"]);

        public static bool IsSynced(this DataRow dataRow) => Utils.ToBoolean(dataRow["IsSync"]);

        public static bool IsDisabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["isDisabled"]);

        public static bool DisableOutright(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableOutright"]);

        public static bool EnableOutright(this DataRow dataRow) => !DisableOutright(dataRow);

        public static bool DisableCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableCasino"]);
        public static bool EnableCasino(this DataRow dataRow) => !DisableCasino(dataRow);

        public static bool DisableNumberGame(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableNumber"]);

        public static bool EnableNumberGame(this DataRow dataRow) => !DisableNumberGame(dataRow);

        public static bool DisableLiveCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableLiveCasino"]);

        public static bool EnableLiveCasino(this DataRow dataRow) => !DisableLiveCasino(dataRow);

        public static bool EnableDoubleComm(this DataRow dataRow) => Utils.ToBoolean(dataRow["doubleCommLosing"]);

        public static bool EnableHorseRacingFixedOdds(this DataRow dataRow) => !Utils.ToBoolean(dataRow["DisableHRFixedOdds"]);

        public static bool DisableESports(this DataRow dataRow) => Utils.ToBoolean(dataRow["disableESports"]);

        public static bool EnableESports(this DataRow dataRow) => !DisableESports(dataRow);

        public static bool DisableColossusBet(this DataRow dataRow) => Utils.ToBoolean(dataRow["DisableColossusBet"]);

        public static bool EnableColossusBet(this DataRow dataRow) => !DisableColossusBet(dataRow);

        public static bool IsSyncedCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["syncCasino"]);

        public static bool IsSyncedLiveCasino(this DataRow dataRow) => Utils.ToBoolean(dataRow["syncLiveCasino"]);

        public static decimal MasterCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["masterCash"]);

        public static decimal AgentCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["agentCash"]);

        public static decimal CustomerCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["customerCash"]);


        public static decimal MasterYesterdayCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["yesterday_masterCash"]);

        public static decimal AgentYesterdayCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["yesterday_agentCash"]);

        public static decimal CustomerYesterdayCash(this DataRow dataRow) => Utils.ToDecimal(dataRow["yesterday_customerCash"]);
    }
}
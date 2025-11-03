namespace Fanex.ExternalAccount.Excel.Extensions
{
    using System;
    using System.Data;
    using Utilities;

    public static class CustomerInfoDataRowExtensions
    {
        internal static string Username(this DataRow dataRow) => dataRow["username"].ToString();

        internal static bool DoubleCommLosing(this DataRow dataRow) => Utils.ToBoolean(dataRow["DoubleCommLosing"]);

        internal static string NickName(this DataRow dataRow) => dataRow["nickname"].ToString();

        internal static string FirstName(this DataRow dataRow) => dataRow["firstname"].ToString();

        internal static string LastName(this DataRow dataRow) => dataRow["lastname"].ToString();

        internal static string UserGroup(this DataRow dataRow) => dataRow["ugroup"].ToString();

        internal static double Discount(this DataRow dataRow) => Utils.ToDouble(dataRow["discount"]);

        internal static double Discount1x2(this DataRow dataRow) => Utils.ToDouble(dataRow["discount1x2"]);

        internal static double DiscountCasino(this DataRow dataRow) => Utils.ToDouble(dataRow["discountcs"]);       

        internal static double PlayerDiscount(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscount"]);

        internal static double PlayerDiscount1x2(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscount1x2"]);

        internal static double PlayerDiscountCasino(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscountcs"]);

        internal static string LastLoginIP(this DataRow dataRow) => dataRow["lastloginip"].ToString();

        internal static string CreateDate(this DataRow dataRow) => $"{dataRow["createdate"]:MM/dd/yyyy HH:mm:ss tt}";

        internal static double GroupA(this DataRow dataRow) => Utils.ToDouble(dataRow["groupa"]);

        internal static double GroupB(this DataRow dataRow) => Utils.ToDouble(dataRow["groupb"]);

        internal static double GroupC(this DataRow dataRow) => Utils.ToDouble(dataRow["groupc"]);

        internal static double GroupD(this DataRow dataRow) => Utils.ToDouble(dataRow["groupd"]);

        internal static bool Enabled(this DataRow dataRow) => Utils.ToBoolean(dataRow["enabled"]);

        internal static bool Closed(this DataRow dataRow) => Utils.ToBoolean(dataRow["closed"]);

        internal static bool Suspended(this DataRow dataRow) => Utils.ToBoolean(dataRow["suspended"]);

        internal static byte ClosedReason(this DataRow dataRow) => Convert.ToByte(dataRow["closedreason"]);

        internal static int? LimitDays(this DataRow dataRow) => dataRow["LimitDays"] == DBNull.Value ? (int?)null : Convert.ToInt32(dataRow["LimitDays"]);

        internal static string MinDate(this DataRow dataRow) => dataRow["MinDate"] == DBNull.Value ? null : Convert.ToDateTime(dataRow["MinDate"]).ToString("MM/dd/yyyy");
    }
}

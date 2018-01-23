namespace Fanex.ExternalAccount.Excel.Extensions
{
    using System.Data;
    using System.Globalization;
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

        internal static double DiscountNumberGame(this DataRow dataRow) => Utils.ToDouble(dataRow["discountnumber"]);

        internal static double PlayerDiscount(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscount"]);

        internal static double PlayerDiscount1x2(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscount1x2"]);

        internal static double PlayerDiscountCasino(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscountcs"]);

        internal static double PlayerDiscountNumberGame(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscountnumber"]);

        internal static string LastLoginIP(this DataRow dataRow) => dataRow["lastloginip"].ToString();

        internal static bool IsMyanmarOdds(this DataRow dataRow) => Utils.ToBoolean(dataRow["ismyanmarodds"]);

        internal static string DiscountMyanmarOdds(this DataRow dataRow) => Utils.ToDouble(dataRow["discountmyanmarodds"]).ToString(CultureInfo.InvariantCulture);

        internal static string PlayerDiscountMyanmarOdds(this DataRow dataRow) => Utils.ToDouble(dataRow["playerdiscountmyanmarodds"]).ToString(CultureInfo.InvariantCulture);

        internal static double GroupA(this DataRow dataRow) => Utils.ToDouble(dataRow["groupa"]);
    }
}

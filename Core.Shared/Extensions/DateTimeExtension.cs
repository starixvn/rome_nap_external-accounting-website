namespace Fanex.ExternalAccount.Core.Shared.Extensions
{
    using System;

    public static class DateTimeExtension
    {
        private const string ShortDate = "MMM dd, yyyy";
        private const string ShortDateWithSlash = "MM/dd/yyyy";
        private const string LongDateTime12h = "MM/dd/yyyy hh:mm tt";

        public static string ToShortDateWithSlash(this DateTime dateTime)
            => dateTime.ToString(ShortDateWithSlash);

        public static string ToShortDateWithSlash(this DateTime? dateTime)
            => dateTime.HasValue ? dateTime.Value.ToShortDateWithSlash() : string.Empty;

        public static string ToLongDateTime12h(this DateTime dateTime)
            => dateTime.ToString(LongDateTime12h);

        public static string ToLongDateTime12h(this DateTime? dateTime)
            => dateTime.HasValue ? dateTime.Value.ToLongDateTime12h() : string.Empty;

        public static string ToShortFormatedDate(this DateTime dateTime)
            => dateTime.ToString(ShortDate);

        public static string ToShortFormatedDate(this DateTime? dateTime)
            => dateTime.HasValue ? dateTime.Value.ToShortFormatedDate() : string.Empty;

        public static DateTime FirstDayOfMonth(this DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dateTime)
            => dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
    }
}

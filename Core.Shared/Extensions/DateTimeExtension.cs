namespace Fanex.ExternalAccount.Core.Shared.Extensions
{
    using System;

    public static class DateTimeExtension
    {
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
    }
}

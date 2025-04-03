namespace Fanex.ProductStatus.Setting.Shared
{
    using System;

    internal static class DateTimeExtensions
    {
        private const string LongDateWithSlash = "dd/MM/yyyy HH:mm";

        public static string ToLongDateWithSlash(this DateTime dateTime)
            => dateTime.ToString(LongDateWithSlash);
    }
}
